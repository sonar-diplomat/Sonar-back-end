using Application.DTOs.Chat;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services;

public interface IMessageService : IGenericService<Message>
{
    Task<Message> CreateAsync(int chatId, int userId, MessageDTO dto);
    Task<IEnumerable<int>> GetMessagesByIdsAsync(IEnumerable<int> messageIds, int chatId);
}