using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Chat;
using Application.Response;
using Entities.Models.Chat;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Chat;

[Route("api/[controller]")]
[ApiController]
public class ChatController(
    UserManager<User> userManager,
    IChatService chatService
) : BaseController(userManager)
{
    [HttpPost("{chatId:int}/send")]
    public async Task<IActionResult> SendMessage(int chatId, [FromBody] MessageDTO message)
    {
        int userId = (await CheckAccessFeatures([])).Id;
        await chatService.SendMessageAsync(userId, chatId, message);
        throw ResponseFactory.Create<OkResponse>(["Message sent successfully"]);
    }

    [HttpDelete("message/{messageId:int}")]
    public async Task<IActionResult> DeleteMessage(int messageId)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.DeleteMessageAsync(user.Id, messageId);
        throw ResponseFactory.Create<OkResponse>(["Message retrieved successfully"]);
    }

    [HttpGet("message/{messageId:int}")]
    public async Task<IActionResult> GetMessageById(int messageId)
    {
        User user = await CheckAccessFeatures([]);
        Message message = await chatService.GetMessageByIdAsync(user.Id, messageId);
        throw ResponseFactory.Create<OkResponse<Message>>(message, ["Message retrieved successfully"]);
    }

    [HttpGet("{chatId:int}/info")]
    public async Task<IActionResult> GetChatInfo(int chatId)
    {
        User user = await CheckAccessFeatures([]);
        ChatDTO chat = await chatService.GetChatInfoAsync(user.Id, chatId);
        throw ResponseFactory.Create<OkResponse<ChatDTO>>(chat, ["Chat info retrieved successfully"]);
    }

    [HttpGet("{chatId:int}/messages")]
    public async Task<IActionResult> GetMessagesWithCursor(int chatId, [FromQuery] int? cursor = null)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{chatId:int}/add/{userId:int}")]
    public async Task<IActionResult> AddUserToChat(int chatId, int userId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{chatId:int}/remove/{userId:int}")]
    public async Task<IActionResult> RemoveUserFromChat(int chatId, int userId)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{chatId:int}/cover")]
    public async Task<IActionResult> UpdateChatCover(int chatId, IFormFile file)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.UpdateChatCoverAsync(user.Id, chatId, file);
        throw ResponseFactory.Create<OkResponse>(["Album cover was updated successfully"]);
    }

    [HttpPut("{chatId:int}/name")]
    public async Task<IActionResult> UpdateChatName(int chatId, string newName)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.UpdateChatNameAsync(user.Id, chatId, newName);
        throw ResponseFactory.Create<OkResponse>(["Album name was updated successfully"]);
    }

    [HttpPut("{chatId:int}read")]
    public async Task<IActionResult> ReadMessages(int chatId, [FromBody] IEnumerable<int> messageIds)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.ReadMessagesAsync(user.Id, chatId, messageIds);
        throw ResponseFactory.Create<OkResponse>(["Messages read successfully"]);
    }

    [HttpPut("{chatId:int}/read-all")]
    public async Task<IActionResult> ReadAllMessages(int chatId)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.ReadAllMessagesAsync(user.Id, chatId);
        throw ResponseFactory.Create<OkResponse>(["All messages marked as read"]);
    }
}