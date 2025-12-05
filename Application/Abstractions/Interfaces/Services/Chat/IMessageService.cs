using Application.DTOs;
using Application.DTOs.Chat;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services;

public interface IMessageService : IGenericService<Message>
{
    Task<Message> CreateAsync(int chatId, int userId, MessageDTO dto);
    Task<IEnumerable<int>> GetMessagesByIdsAsync(IEnumerable<int> messageIds, int chatId);
    Task<CursorPageDTO<MessageDTO>> GetMessagesWithCursorAsync(int chatId, int? cursor, int take = 50);
    Task<Dictionary<int, LastMessageDTO?>> GetLastMessagesForChatsAsync(IEnumerable<int> chatIds);
}