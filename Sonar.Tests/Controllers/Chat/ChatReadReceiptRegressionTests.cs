using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sonar.Controllers.Chat;
using Sonar.Tests.Services.Chat;
using Xunit;

namespace Sonar.Tests.Controllers.Chat;

public class ChatReadReceiptRegressionTests : ChatControllerTestsBase
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadMessages_DistinctChatAndUserIds_RecordsOnlyCallerAndRequestedChat(bool includeForeignMessage)
    {
        using var fixture = new ChatRegressionFixture();
        var intendedChat = await fixture.SeedChat(chatId: 7, ownerId: 1, memberIds: [1]);
        var foreignChat = await fixture.SeedChat(chatId: 1, ownerId: 7, memberIds: [7]);
        fixture.AddMessage(intendedChat, 100, 1);
        fixture.AddMessage(foreignChat, 200, 7);
        await fixture.Db.SaveChangesAsync();
        using var manager = CreateUserManager(fixture.Db);
        var controller = new ChatController(manager, fixture.Service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal(intendedChat.Creator) }
            }
        };

        await Assert.ThrowsAsync<OkResponse>(() => controller.ReadMessages(7, includeForeignMessage ? [100, 200] : [100]));

        var read = Assert.Single(await fixture.Db.MessageReads.ToListAsync());
        Assert.Equal(1, read.UserId);
        Assert.Equal(100, read.MessageId);
        fixture.Notifier.Verify(notifier => notifier.MessagesRead(It.Is<MessagesReadEvent>(evt =>
            evt.ChatId == 7 && evt.UserId == 1 && evt.MessageIds.SequenceEqual(new[] { 100 }))), Times.Once);
    }

    [Fact]
    public async Task ReadMessages_OnlyForeignMessage_DoesNotWriteReceiptForEitherUser()
    {
        using var fixture = new ChatRegressionFixture();
        var intendedChat = await fixture.SeedChat(chatId: 7, ownerId: 1, memberIds: [1]);
        var foreignChat = await fixture.SeedChat(chatId: 1, ownerId: 7, memberIds: [7]);
        fixture.AddMessage(foreignChat, 200, 7);
        await fixture.Db.SaveChangesAsync();
        using var manager = CreateUserManager(fixture.Db);
        var controller = new ChatController(manager, fixture.Service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal(intendedChat.Creator) }
            }
        };

        await Assert.ThrowsAsync<ForbiddenResponse>(() => controller.ReadMessages(7, [200]));

        Assert.False(await fixture.Db.MessageReads.AnyAsync());
        fixture.Notifier.VerifyNoOtherCalls();
    }
}
