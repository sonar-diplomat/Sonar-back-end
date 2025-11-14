using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Chat;
using Application.Response;
using Entities.Models.Chat;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
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
    /// <summary>
    /// Sends a message to a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat to send the message to.</param>
    /// <param name="message">The message DTO containing content and type.</param>
    /// <returns>Success response upon message sent.</returns>
    /// <response code="200">Message sent successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Chat not found or user not a member.</response>
    [HttpPost("{chatId:int}/send")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessage(int chatId, [FromBody] MessageDTO message)
    {
        int userId = (await CheckAccessFeatures([])).Id;
        await chatService.SendMessageAsync(userId, chatId, message);
        throw ResponseFactory.Create<OkResponse>(["Message sent successfully"]);
    }

    /// <summary>
    /// Deletes a message from a chat.
    /// </summary>
    /// <param name="messageId">The ID of the message to delete.</param>
    /// <returns>Success response upon message deletion.</returns>
    /// <response code="200">Message deleted successfully.</response>
    /// <response code="401">User not authenticated or not the message author.</response>
    /// <response code="404">Message not found.</response>
    [HttpDelete("message/{messageId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMessage(int messageId)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.DeleteMessageAsync(user.Id, messageId);
        throw ResponseFactory.Create<OkResponse>(["Message retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves a specific message by its ID.
    /// </summary>
    /// <param name="messageId">The ID of the message to retrieve.</param>
    /// <returns>Message entity with full details.</returns>
    /// <response code="200">Message retrieved successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Message not found.</response>
    [HttpGet("message/{messageId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<Message>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessageById(int messageId)
    {
        User user = await CheckAccessFeatures([]);
        Message message = await chatService.GetMessageByIdAsync(user.Id, messageId);
        throw ResponseFactory.Create<OkResponse<Message>>(message, ["Message retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves information about a specific chat including members and metadata.
    /// </summary>
    /// <param name="chatId">The ID of the chat to retrieve information for.</param>
    /// <returns>Chat DTO with member list and chat details.</returns>
    /// <response code="200">Chat information retrieved successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Chat not found.</response>
    [HttpGet("{chatId:int}/info")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<ChatDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChatInfo(int chatId)
    {
        User user = await CheckAccessFeatures([]);
        ChatDTO chat = await chatService.GetChatInfoAsync(user.Id, chatId);
        throw ResponseFactory.Create<OkResponse<ChatDTO>>(chat, ["Chat info retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves messages from a chat using cursor-based pagination.
    /// </summary>
    /// <param name="chatId">The ID of the chat to retrieve messages from.</param>
    /// <param name="cursor">Optional cursor for pagination (message ID to start from).</param>
    /// <param name="take">Number of messages to retrieve (default: 50).</param>
    /// <returns>Cursor-paginated list of message DTOs.</returns>
    /// <response code="200">Messages retrieved successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Chat not found.</response>
    [HttpGet("{chatId:int}/messages")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<CursorPageDTO<MessageDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessagesWithCursor(int chatId, [FromQuery] int? cursor = null,
        [FromQuery] int take = 50)
    {
        User user = await CheckAccessFeatures([]);
        CursorPageDTO<MessageDTO> page = await chatService.GetMessagesWithCursorAsync(user.Id, chatId, cursor, take);
        throw ResponseFactory.Create<OkResponse<CursorPageDTO<MessageDTO>>>(page, ["Messages retrieved successfully"]);
    }

    /// <summary>
    /// Adds a user to a chat (requires chat membership).
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="userId">The ID of the user to add.</param>
    /// <returns>Success response upon adding user.</returns>
    /// <response code="200">User added to chat successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Chat or user not found.</response>
    [HttpPost("{chatId:int}/add/{userId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserToChat(int chatId, int userId)
    {
        int chatMemberId = (await CheckAccessFeatures([])).Id;
        await chatService.AddUserToChat(chatMemberId, chatId, userId);
        throw ResponseFactory.Create<OkResponse>(["User added successfully"]);
    }

    /// <summary>
    /// Allows the authenticated user to leave a chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat to leave.</param>
    /// <returns>Success response upon leaving the chat.</returns>
    /// <response code="200">Successfully left the chat.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Chat not found.</response>
    [HttpDelete("{chatId:int}/leave")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LeaveChat(int chatId)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.LeaveChat(user.Id, chatId);
        throw ResponseFactory.Create<OkResponse>(["Chat leaved successfully"]);
    }

    /// <summary>
    /// Removes a user from a chat (requires appropriate permissions).
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <returns>Success response upon removing user.</returns>
    /// <response code="200">User removed from chat successfully.</response>
    /// <response code="401">User not authenticated or lacks permissions.</response>
    /// <response code="404">Chat or user not found.</response>
    [HttpDelete("{chatId:int}/remove/{userId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUserFromChat(int chatId, int userId)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.RemoveUserFromChat(user.Id, userId, chatId);
        throw ResponseFactory.Create<OkResponse>(["User removed from chat successfully"]);
    }

    /// <summary>
    /// Updates the cover image of a chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="file">The new cover image file.</param>
    /// <returns>Success response upon cover update.</returns>
    /// <response code="200">Chat cover updated successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="400">Invalid image file.</response>
    /// <response code="404">Chat not found.</response>
    [HttpPut("{chatId:int}/cover")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateChatCover(int chatId, IFormFile file)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.UpdateChatCoverAsync(user.Id, chatId, file);
        throw ResponseFactory.Create<OkResponse>(["Album cover was updated successfully"]);
    }

    /// <summary>
    /// Updates the name of a chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="newName">The new name for the chat.</param>
    /// <returns>Success response upon name update.</returns>
    /// <response code="200">Chat name updated successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Chat not found.</response>
    [HttpPut("{chatId:int}/name")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateChatName(int chatId, string newName)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.UpdateChatNameAsync(user.Id, chatId, newName);
        throw ResponseFactory.Create<OkResponse>(["Album name was updated successfully"]);
    }

    /// <summary>
    /// Marks specific messages as read in a chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="messageIds">Collection of message IDs to mark as read.</param>
    /// <returns>Success response upon marking messages as read.</returns>
    /// <response code="200">Messages marked as read successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Chat or messages not found.</response>
    [HttpPut("{chatId:int}/read")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadMessages(int chatId, [FromBody] IEnumerable<int> messageIds)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.ReadMessagesAsync(user.Id, chatId, messageIds);
        throw ResponseFactory.Create<OkResponse>(["Messages read successfully"]);
    }

    /// <summary>
    /// Marks all messages in a chat as read for the authenticated user.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <returns>Success response upon marking all messages as read.</returns>
    /// <response code="200">All messages marked as read successfully.</response>
    /// <response code="401">User not authenticated or not a chat member.</response>
    /// <response code="404">Chat not found.</response>
    [HttpPut("{chatId:int}/read-all")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadAllMessages(int chatId)
    {
        User user = await CheckAccessFeatures([]);
        await chatService.ReadAllMessagesAsync(user.Id, chatId);
        throw ResponseFactory.Create<OkResponse>(["All messages marked as read"]);
    }
}