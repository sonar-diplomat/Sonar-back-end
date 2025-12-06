using Application.DTOs;
using Application.DTOs.Chat;
using Entities.Models.Chat;
using Microsoft.AspNetCore.Http;
using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Abstractions.Interfaces.Services;

public interface IChatService : IGenericService<ChatModel>
{
    Task UpdateChatCoverAsync(int userId, int chatId, IFormFile file);
    Task UpdateChatNameAsync(int userId, int chatId, string newName);
    Task<Message> GetMessageByIdAsync(int userId, int messageId);
    Task DeleteMessageAsync(int userId, int messageId);
    Task<Message> EditMessageAsync(int userId, int messageId, EditMessageDTO dto);
    Task<Message> SendMessageAsync(int userId, int chatId, MessageDTO message);
    Task<ChatDTO> GetChatInfoAsync(int userId, int chatId);
    Task ReadMessagesAsync(int chatId, int userId, IEnumerable<int> messageIds);
    Task ReadAllMessagesAsync(int userId, int chatId);
    Task AddUserToChat(int memberId, int chatId, int userId);
    Task RemoveUserFromChat(int initiatorId, int userId, int chatId);
    Task<CursorPageDTO<MessageDTO>> GetMessagesWithCursorAsync(int userId, int chatId, int? cursor, int take = 50);
    Task LeaveChat(int userId, int chatId);
    Task<ChatModel> CreateChatAsync(int userId, CreateChatDTO chat);
    Task<IEnumerable<ChatListItemDTO>> GetUserChatsAsync(int userId);
}