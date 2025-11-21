using Application.Abstractions.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Test;

[ApiController]
[Route("api/dev/chat-test")]
[AllowAnonymous]
public sealed class ChatEventsTestController(IChatNotifier notifier) : ControllerBase
{

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Chat test endpoints are alive");
    }

    [HttpPost("message-created")]
    public async Task<IActionResult> MessageCreated([FromBody] MessageCreatedEvent e)
    {
        await notifier.MessageCreated(e);
        return Ok(new { sent = "message.created", e.ChatId, e.Id });
    }

    [HttpPost("message-deleted")]
    public async Task<IActionResult> MessageDeleted([FromBody] MessageDeletedEvent e)
    {
        await notifier.MessageDeleted(e);
        return Ok(new { sent = "message.deleted", e.ChatId, e.MessageId });
    }

    [HttpPost("messages-read")]
    public async Task<IActionResult> MessagesRead([FromBody] MessagesReadEvent e)
    {
        await notifier.MessagesRead(e);
        return Ok(new { sent = "message.read", e.ChatId, count = e.MessageIds.Count });
    }

    [HttpPost("chat-name-updated")]
    public async Task<IActionResult> ChatNameUpdated([FromBody] ChatNameUpdatedEvent e)
    {
        await notifier.ChatNameUpdated(e);
        return Ok(new { sent = "chat.name.updated", e.ChatId, e.Name });
    }

    [HttpPost("chat-cover-updated")]
    public async Task<IActionResult> ChatCoverUpdated([FromBody] ChatCoverUpdatedEvent e)
    {
        await notifier.ChatCoverUpdated(e);
        return Ok(new { sent = "chat.cover.updated", e.ChatId, e.CoverId });
    }

    [HttpPost("user-added")]
    public async Task<IActionResult> UserAdded([FromBody] UserAddedToChatEvent e)
    {
        await notifier.UserAdded(e);
        return Ok(new { sent = "chat.user.added", e.ChatId, e.UserId });
    }

    [HttpPost("user-removed")]
    public async Task<IActionResult> UserRemoved([FromBody] UserRemovedFromChatEvent e)
    {
        await notifier.UserRemoved(e);
        return Ok(new { sent = "chat.user.removed", e.ChatId, e.UserId });
    }
    
    [HttpPost("all")]
    public async Task<IActionResult> FireAll([FromQuery] int chatId, [FromQuery] int userId)
    {
        DateTime now = DateTime.UtcNow;

        await notifier.MessageCreated(new MessageCreatedEvent(
            1001,
            chatId,
            userId,
            "🔥 Test message from dev endpoint",
            null,
            now
        ));

        await notifier.MessagesRead(new MessagesReadEvent(
            chatId,
            userId,
            new List<int> { 1001, 1002, 1003 },
            now.AddSeconds(1)
        ));

        await notifier.ChatNameUpdated(new ChatNameUpdatedEvent(
            chatId,
            $"Dev Chat {now:HH:mm:ss}"
        ));

        await notifier.ChatCoverUpdated(new ChatCoverUpdatedEvent(
            chatId,
            777
        ));

        await notifier.UserAdded(new UserAddedToChatEvent(
            chatId,
            9991
        ));

        await notifier.UserRemoved(new UserRemovedFromChatEvent(
            chatId,
            9991
        ));

        await notifier.MessageDeleted(new MessageDeletedEvent(
            chatId,
            1001,
            userId
        ));

        return Ok(new { sent = "all", chatId, userId });
    }
}