using Application.DTOs;
using Application.DTOs.Chat;
using Entities.Models.Chat;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services;

public interface IChatService : IGenericService<Chat>
{
    Task UpdateChatCoverAsync(int userId, int chatId, IFormFile file);
    Task UpdateChatNameAsync(int userId, int chatId, string newName);
    Task<Message> GetMessageByIdAsync(int userId, int messageId);
    Task DeleteMessageAsync(int userId, int messageId);
    Task<Message> SendMessageAsync(int userId, int chatId, MessageDTO message);
    Task<ChatDTO> GetChatInfoAsync(int userId, int chatId);
    Task ReadMessagesAsync(int chatId, int userId, IEnumerable<int> messageIds);
    Task ReadAllMessagesAsync(int userId, int chatId);
    Task AddUserToChat(int memberId, int chatId, int userId);
    Task RemoveUserFromChat(int initiatorId, int userId, int chatId);
    Task<CursorPageDTO<MessageDTO>> GetMessagesWithCursorAsync(int userId, int chatId, int? cursor, int take = 50);
    Task LeaveChat(int userId, int chatId);
    Task<Chat> CreateChatAsync(int userId, CreateChatDTO chat);
}