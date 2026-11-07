using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.Services.Chat;
using Entities.Models.Chat;
using Entities.Models.File;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Infrastructure.Repository.Chat;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sonar.Infrastructure.Repository.Chat;
using Sonar.Tests.Controllers.Chat;
using ChatModel = Entities.Models.Chat.Chat;

namespace Sonar.Tests.Services.Chat;

internal sealed class ChatRegressionFixture : ChatControllerTestsBase, IDisposable
{
    public SonarContext Db { get; }
    public ChatService Service { get; }
    public Mock<IChatNotifier> Notifier { get; } = new();

    public ChatRegressionFixture()
    {
        Db = CreateInMemoryContext();
        var users = new Mock<IUserRepository>();
        users.Setup(repository => repository.Query()).Returns(() => Db.Users);
        var userService = new Mock<IUserService>();
        userService.Setup(service => service.GetByIdValidatedAsync(It.IsAny<int>()))
            .Returns((int id) => Db.Users.SingleAsync(user => user.Id == id));
        var reads = new MessageReadService(new MessageReadRepository(Db));
        var messages = new MessageService(new MessageRepository(Db), reads, users.Object);
        Service = new ChatService(new ChatRepository(Db), Mock.Of<IImageFileService>(),
            messages, reads, userService.Object, Notifier.Object, Mock.Of<IUserFollowService>(), users.Object);
    }

    public async Task<ChatModel> SeedChat(int chatId = 90, int ownerId = 11,
        int[]? memberIds = null, int[]? adminIds = null)
    {
        memberIds ??= [ownerId, 22, 33];
        var members = new List<User>();
        foreach (int id in memberIds)
        {
            User? user = await Db.Users.FindAsync(id);
            if (user == null)
            {
                user = CreateTestUser(id);
                user.AccessFeatures = Db.AccessFeatures.Local.ToList();
                if (user.AccessFeatures.Count == 0)
                    user.AccessFeatures = CreateTestUser(id).AccessFeatures;
                Db.Users.Add(user);
            }
            members.Add(user);
        }
        var chat = new ChatModel
        {
            Id = chatId, Name = "Regression group", IsGroup = true,
            CreatorId = ownerId, Creator = members.Single(user => user.Id == ownerId),
            Cover = new ImageFile { ItemName = "Cover", Url = "https://example.test/cover.png" },
            Members = members, Admins = members.Where(user => (adminIds ?? []).Contains(user.Id)).ToList(),
            Messages = []
        };
        Db.Chats.Add(chat);
        await Db.SaveChangesAsync();
        return chat;
    }

    public async Task<ChatModel> ReloadChat(int chatId = 90)
    {
        Db.ChangeTracker.Clear();
        return await Db.Chats.Include(chat => chat.Members).Include(chat => chat.Admins)
            .Include(chat => chat.Creator).SingleAsync(chat => chat.Id == chatId);
    }

    public Message AddMessage(ChatModel chat, int messageId, int senderId)
    {
        var message = new Message
        {
            Id = messageId, Chat = chat, SenderId = senderId, TextContent = "A message",
            CreatedAt = DateTime.UtcNow, MessagesReads = []
        };
        Db.Messages.Add(message);
        return message;
    }

    public void Dispose() => Db.Dispose();
}
