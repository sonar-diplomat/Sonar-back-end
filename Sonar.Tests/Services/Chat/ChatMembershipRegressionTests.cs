using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Chat;
using Application.Response;
using Entities.Models.Chat;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Sonar.Tests.Services.Chat;

public class ChatMembershipRegressionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CreatorRemovesMember_PersistsMembershipAndAdminRemovalAndNotifies(bool isAdmin)
    {
        using var fixture = new ChatRegressionFixture();
        await fixture.SeedChat(adminIds: isAdmin ? [22] : []);

        await fixture.Service.RemoveUserFromChat(11, 22, 90);

        var chat = await fixture.ReloadChat();
        Assert.DoesNotContain(chat.Members, user => user.Id == 22);
        Assert.DoesNotContain(chat.Admins, user => user.Id == 22);
        Assert.Equal(11, chat.CreatorId);
        fixture.Notifier.Verify(notifier => notifier.UserRemoved(new UserRemovedFromChatEvent(90, 22)), Times.Once);
    }

    [Fact]
    public async Task AdministratorCanRemoveOrdinaryMember()
    {
        using var fixture = new ChatRegressionFixture();
        await fixture.SeedChat(adminIds: [22]);

        await fixture.Service.RemoveUserFromChat(22, 33, 90);

        var chat = await fixture.ReloadChat();
        Assert.DoesNotContain(chat.Members, user => user.Id == 33);
        Assert.Contains(chat.Admins, user => user.Id == 22);
        fixture.Notifier.Verify(notifier => notifier.UserRemoved(new UserRemovedFromChatEvent(90, 33)), Times.Once);
    }

    [Theory]
    [InlineData(33, 22)] // An ordinary member cannot remove another member.
    [InlineData(22, 33)] // An administrator cannot remove another administrator.
    [InlineData(22, 11)] // An administrator cannot remove the creator.
    [InlineData(44, 22)] // A nonmember cannot use management privileges.
    public async Task UnauthorizedRemoval_PreservesMembershipAndDoesNotNotify(int initiatorId, int targetId)
    {
        using var fixture = new ChatRegressionFixture();
        await fixture.SeedChat(adminIds: initiatorId == 33 ? [22] : [22, 33]);

        await Assert.ThrowsAsync<ForbiddenResponse>(() => fixture.Service.RemoveUserFromChat(initiatorId, targetId, 90));

        var chat = await fixture.ReloadChat();
        Assert.Equal(new[] { 11, 22, 33 }, chat.Members.Select(user => user.Id).Order().ToArray());
        Assert.Equal(11, chat.CreatorId);
        fixture.Notifier.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MemberLeaves_RemovesTheirMembershipAndAdminRoleAndNotifies(bool isAdmin)
    {
        using var fixture = new ChatRegressionFixture();
        await fixture.SeedChat(adminIds: isAdmin ? [22] : []);

        await fixture.Service.LeaveChat(22, 90);

        var chat = await fixture.ReloadChat();
        Assert.DoesNotContain(chat.Members, user => user.Id == 22);
        Assert.DoesNotContain(chat.Admins, user => user.Id == 22);
        Assert.Equal(11, chat.CreatorId);
        fixture.Notifier.Verify(notifier => notifier.UserRemoved(new UserRemovedFromChatEvent(90, 22)), Times.Once);
    }

    [Theory]
    [InlineData(true, 33)]
    [InlineData(false, 22)]
    public async Task CreatorLeaves_TransfersOwnershipToRemainingAdminThenLowestIdMember(bool hasRemainingAdmin, int successorId)
    {
        using var fixture = new ChatRegressionFixture();
        await fixture.SeedChat(memberIds: [11, 44, 33, 22], adminIds: hasRemainingAdmin ? [11, 44, 33] : [11]);

        await fixture.Service.LeaveChat(11, 90);

        var chat = await fixture.ReloadChat();
        Assert.Equal(successorId, chat.CreatorId);
        Assert.Equal(successorId, chat.Creator.Id);
        Assert.Contains(chat.Members, user => user.Id == chat.CreatorId);
        Assert.DoesNotContain(chat.Members, user => user.Id == 11);
        Assert.DoesNotContain(chat.Admins, user => user.Id == 11);
        Assert.All(chat.Admins, admin => Assert.Contains(chat.Members, user => user.Id == admin.Id));
        fixture.Notifier.Verify(notifier => notifier.UserRemoved(new UserRemovedFromChatEvent(90, 11)), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LastMemberLeavesOrRemovesSelf_DeletesChatReadsAndMessagesAndNotifiesFormerMember(bool removeSelf)
    {
        using var fixture = new ChatRegressionFixture();
        var chat = await fixture.SeedChat(memberIds: [11], adminIds: [11]);
        var message = fixture.AddMessage(chat, 101, 11);
        fixture.Db.MessageReads.Add(new MessageRead { Message = message, UserId = 11, ReadAt = DateTime.UtcNow });
        await fixture.Db.SaveChangesAsync();

        if (removeSelf) await fixture.Service.RemoveUserFromChat(11, 11, 90);
        else await fixture.Service.LeaveChat(11, 90);

        fixture.Db.ChangeTracker.Clear();
        Assert.False(await fixture.Db.Chats.AnyAsync());
        Assert.False(await fixture.Db.Messages.AnyAsync());
        Assert.False(await fixture.Db.MessageReads.AnyAsync());
        fixture.Notifier.Verify(notifier => notifier.ChatDeleted(new ChatDeletedEvent(90, 11),
            It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 11 }))), Times.Once);
        fixture.Notifier.Verify(notifier => notifier.UserRemoved(new UserRemovedFromChatEvent(90, 11)), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FormerMember_CannotReadSendEditDeleteOrMarkMessagesRead(bool leaveVoluntarily)
    {
        using var fixture = new ChatRegressionFixture();
        var chat = await fixture.SeedChat();
        fixture.AddMessage(chat, 101, 22);
        await fixture.Db.SaveChangesAsync();
        if (leaveVoluntarily) await fixture.Service.LeaveChat(22, 90);
        else await fixture.Service.RemoveUserFromChat(11, 22, 90);
        fixture.Notifier.Invocations.Clear();

        await Assert.ThrowsAsync<ForbiddenResponse>(() => fixture.Service.GetMessageByIdAsync(22, 101));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => fixture.Service.SendMessageAsync(22, 90, new MessageDTO { TextContent = "Denied" }));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => fixture.Service.EditMessageAsync(22, 101, new EditMessageDTO { TextContent = "Denied" }));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => fixture.Service.DeleteMessageAsync(22, 101));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => fixture.Service.ReadMessagesAsync(90, 22, [101]));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => fixture.Service.ReadAllMessagesAsync(22, 90));
        Assert.Equal("A message", (await fixture.Db.Messages.SingleAsync()).TextContent);
        Assert.False(await fixture.Db.MessageReads.AnyAsync());
        fixture.Notifier.VerifyNoOtherCalls();
    }
}
