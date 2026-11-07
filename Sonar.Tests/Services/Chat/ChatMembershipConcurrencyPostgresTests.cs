using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.Services.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;
using Infrastructure.Repository.Chat;
using Microsoft.EntityFrameworkCore;
using Moq;
using Npgsql;
using Sonar.Infrastructure.Repository.Chat;
using Sonar.Tests.Repositories.UserCore;
using Xunit;
using ChatModel = Entities.Models.Chat.Chat;

namespace Sonar.Tests.Services.Chat;

/// <summary>
/// Uses independent PostgreSQL connections and preloaded EF tracking graphs to
/// exercise the concurrency boundary used by separate application instances.
/// </summary>
public sealed class ChatMembershipConcurrencyPostgresTests(
    ChatMembershipConcurrencyPostgresTests.PostgresFixture fixture)
    : IClassFixture<ChatMembershipConcurrencyPostgresTests.PostgresFixture>
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConcurrentDeparturesRecomputeSuccessorAndLastMemberDeletion(bool hasRemainingMember)
    {
        int chatId = hasRemainingMember ? 701 : 702;
        await fixture.SeedChat(chatId, hasRemainingMember);
        await using var firstContext = fixture.CreateContext();
        await using var secondContext = fixture.CreateContext();
        await firstContext.Database.OpenConnectionAsync();
        await secondContext.Database.OpenConnectionAsync();

        // Both requests start with the original graph, including the soon-to-leave
        // successor 22. Acquiring the database lock must invalidate this stale state.
        await LoadChat(firstContext, chatId);
        await LoadChat(secondContext, chatId);
        var firstRepository = new PausingChatRepository(firstContext);
        var firstNotifier = CreateCommittedStateNotifier(firstContext, chatId);
        var secondNotifier = CreateCommittedStateNotifier(secondContext, chatId);
        var firstService = CreateService(firstContext, firstRepository, firstNotifier.Object);
        var secondService = CreateService(secondContext, new ChatRepository(secondContext), secondNotifier.Object);

        Task firstDeparture = firstService.LeaveChat(22, chatId);
        Task? secondDeparture = null;
        try
        {
            // Member 22 has loaded and changed its graph but has not saved yet.
            // Keep its database lock open while the owner starts a second request.
            await firstRepository.BeforeSave.Task.WaitAsync(TimeSpan.FromSeconds(15));
            Assert.False(firstDeparture.IsCompleted);
            firstNotifier.VerifyNoOtherCalls();
            secondDeparture = secondService.LeaveChat(11, chatId);

            int blockingPid = ((NpgsqlConnection)firstContext.Database.GetDbConnection()).ProcessID;
            int waitingPid = ((NpgsqlConnection)secondContext.Database.GetDbConnection()).ProcessID;
            Assert.NotEqual(blockingPid, waitingPid);
            await fixture.WaitForDatabaseBlock(blockingPid, waitingPid, secondDeparture);
            secondNotifier.VerifyNoOtherCalls();
        }
        finally
        {
            firstRepository.ContinueSave.TrySetResult();
            await firstDeparture.WaitAsync(TimeSpan.FromSeconds(15));
            if (secondDeparture != null)
                await secondDeparture.WaitAsync(TimeSpan.FromSeconds(15));
        }

        await using var verification = fixture.CreateContext();
        var finalChat = await verification.Chats.Include(chat => chat.Members)
            .Include(chat => chat.Admins).SingleOrDefaultAsync(chat => chat.Id == chatId);
        if (hasRemainingMember)
        {
            Assert.NotNull(finalChat);
            Assert.Equal(new[] { 33 }, finalChat.Members.Select(user => user.Id).ToArray());
            Assert.Equal(33, finalChat.CreatorId);
            Assert.Empty(finalChat.Admins);
            Assert.True(await verification.Messages.AnyAsync(message => message.ChatId == chatId));
            secondNotifier.Verify(notifier => notifier.ChatDeleted(It.IsAny<ChatDeletedEvent>(),
                It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
        }
        else
        {
            Assert.Null(finalChat);
            Assert.False(await verification.Messages.AnyAsync(message => message.ChatId == chatId));
            Assert.False(await verification.MessageReads.AnyAsync(read => read.MessageId == chatId));
            secondNotifier.Verify(notifier => notifier.ChatDeleted(new ChatDeletedEvent(chatId, 11),
                It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 11 }))), Times.Once);
        }
        firstNotifier.Verify(notifier => notifier.UserRemoved(new UserRemovedFromChatEvent(chatId, 22)), Times.Once);
        secondNotifier.Verify(notifier => notifier.UserRemoved(new UserRemovedFromChatEvent(chatId, 11)), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailureAfterSavingRollsBackStateAndSuppressesNotifications(bool deleteEntireChat)
    {
        int chatId = deleteEntireChat ? 703 : 704;
        await fixture.SeedChat(chatId, hasRemainingMember: true);
        await using var writer = fixture.CreateContext();
        var notifier = new Mock<IChatNotifier>(MockBehavior.Strict);
        var service = CreateService(writer, new FailingAfterSaveChatRepository(writer), notifier.Object);

        // The exception occurs after real SQL writes, before the transaction can
        // commit. Cover both successor transfer and deletion of messages/receipts.
        await Assert.ThrowsAsync<SimulatedWriteFailure>(() => deleteEntireChat
            ? service.DeleteChatAsync(11, chatId)
            : service.LeaveChat(11, chatId));

        Assert.Null(writer.Database.CurrentTransaction);
        Assert.Empty(writer.ChangeTracker.Entries());
        notifier.VerifyNoOtherCalls();
        await using var verification = fixture.CreateContext();
        var chat = await LoadChat(verification, chatId);
        Assert.Equal(11, chat.CreatorId);
        Assert.Equal(new[] { 11, 22, 33 }, chat.Members.Select(user => user.Id).Order().ToArray());
        Assert.Equal(new[] { 11, 22 }, chat.Admins.Select(user => user.Id).Order().ToArray());
        Assert.True(await verification.Messages.AnyAsync(message => message.ChatId == chatId));
        Assert.True(await verification.MessageReads.AnyAsync(read => read.MessageId == chatId));
    }

    private Mock<IChatNotifier> CreateCommittedStateNotifier(SonarContext writer, int chatId)
    {
        var notifier = new Mock<IChatNotifier>(MockBehavior.Strict);
        notifier.Setup(value => value.UserRemoved(It.IsAny<UserRemovedFromChatEvent>()))
            .Returns(async (UserRemovedFromChatEvent payload) =>
            {
                Assert.Null(writer.Database.CurrentTransaction);
                await using var observer = fixture.CreateContext();
                var chat = await observer.Chats.Include(value => value.Members)
                    .SingleOrDefaultAsync(value => value.Id == chatId);
                if (chat != null)
                {
                    Assert.DoesNotContain(chat.Members, member => member.Id == payload.UserId);
                    Assert.Contains(chat.Members, member => member.Id == chat.CreatorId);
                }
            });
        notifier.Setup(value => value.ChatDeleted(It.IsAny<ChatDeletedEvent>(), It.IsAny<IReadOnlyCollection<int>>()))
            .Returns(async (ChatDeletedEvent payload, IReadOnlyCollection<int> _) =>
            {
                Assert.Null(writer.Database.CurrentTransaction);
                await using var observer = fixture.CreateContext();
                Assert.False(await observer.Chats.AnyAsync(chat => chat.Id == payload.ChatId));
                Assert.False(await observer.Messages.AnyAsync(message => message.ChatId == payload.ChatId));
                Assert.False(await observer.MessageReads.AnyAsync(read => read.MessageId == chatId));
            });
        return notifier;
    }

    private static Task<ChatModel> LoadChat(SonarContext context, int chatId) =>
        context.Chats.Include(chat => chat.Members).Include(chat => chat.Admins)
            .Include(chat => chat.Creator).SingleAsync(chat => chat.Id == chatId);

    private static ChatService CreateService(SonarContext context, ChatRepository repository, IChatNotifier notifier)
    {
        var users = new Mock<IUserRepository>();
        users.Setup(value => value.Query()).Returns(() => context.Users);
        var userService = new Mock<IUserService>();
        userService.Setup(value => value.GetByIdValidatedAsync(It.IsAny<int>()))
            .Returns((int id) => context.Users.SingleAsync(user => user.Id == id));
        var reads = new MessageReadService(new MessageReadRepository(context));
        var messages = new MessageService(new MessageRepository(context), reads, users.Object);
        return new ChatService(repository, Mock.Of<IImageFileService>(), messages, reads,
            userService.Object, notifier, Mock.Of<IUserFollowService>(), users.Object);
    }

    private sealed class PausingChatRepository(SonarContext context) : ChatRepository(context)
    {
        public TaskCompletionSource BeforeSave { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource ContinueSave { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override async Task SaveChangesAsync()
        {
            BeforeSave.TrySetResult();
            await ContinueSave.Task.WaitAsync(TimeSpan.FromSeconds(30));
            await base.SaveChangesAsync();
        }

        // Retain the gate for the former graph-update path as well, so removing
        // serialization does not silently stop this regression reaching its race.
        public override async Task<ChatModel> UpdateAsync(ChatModel chat)
        {
            BeforeSave.TrySetResult();
            await ContinueSave.Task.WaitAsync(TimeSpan.FromSeconds(30));
            return await base.UpdateAsync(chat);
        }
    }

    private sealed class SimulatedWriteFailure : Exception { }

    private sealed class FailingAfterSaveChatRepository(SonarContext context) : ChatRepository(context)
    {
        public override async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
            throw new SimulatedWriteFailure();
        }

        public override async Task RemoveAsync(ChatModel chat)
        {
            await base.RemoveAsync(chat);
            throw new SimulatedWriteFailure();
        }
    }

    public sealed class PostgresFixture : DatabaseTestBase
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            foreach (int userId in new[] { 11, 22, 33 })
            {
                await CreateUserDependenciesAsync(userId);
                DbContext!.Users.Add(CreateTestUser(userId, $"race{userId}", $"race{userId}@example.invalid"));
            }
            await DbContext!.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();
        }

        public SonarContext CreateContext() => new(new DbContextOptionsBuilder<SonarContext>()
            .UseNpgsql(ConnectionString).Options);

        public async Task SeedChat(int chatId, bool hasRemainingMember)
        {
            await using var context = CreateContext();
            var users = await context.Users.Where(user => user.Id == 11 || user.Id == 22 ||
                (hasRemainingMember && user.Id == 33)).OrderBy(user => user.Id).ToListAsync();
            var chat = new ChatModel
            {
                Id = chatId, Name = "Concurrent departures", IsGroup = true,
                CreatorId = 11, CoverId = 1, Members = users,
                Admins = users.Where(user => user.Id != 33).ToList(), Messages = []
            };
            context.Chats.Add(chat);
            var message = new Message
            {
                Id = chatId, Chat = chat, SenderId = 11,
                TextContent = "Persisted before departures", CreatedAt = DateTime.UtcNow,
                MessagesReads = []
            };
            context.Messages.Add(message);
            context.MessageReads.Add(new MessageRead { Message = message, UserId = 22, ReadAt = DateTime.UtcNow });
            await context.SaveChangesAsync();
        }

        public async Task WaitForDatabaseBlock(int blockingPid, int waitingPid, Task waitingOperation)
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await using var observer = new NpgsqlConnection(ConnectionString);
            await observer.OpenAsync(deadline.Token);
            await using var command = new NpgsqlCommand("SELECT @blocker = ANY (pg_blocking_pids(@waiter))", observer);
            command.Parameters.AddWithValue("blocker", blockingPid);
            command.Parameters.AddWithValue("waiter", waitingPid);
            while (!deadline.IsCancellationRequested)
            {
                if (waitingOperation.IsCompleted)
                {
                    await waitingOperation;
                    Assert.Fail("The second departure completed while the first departure was paused before saving.");
                }
                if ((bool)(await command.ExecuteScalarAsync(deadline.Token))!) return;
                // Poll a database-visible wait condition, not an assumed task schedule.
                await Task.Delay(20, deadline.Token);
            }
            Assert.Fail("PostgreSQL did not report the second connection waiting for the first connection.");
        }
    }
}
