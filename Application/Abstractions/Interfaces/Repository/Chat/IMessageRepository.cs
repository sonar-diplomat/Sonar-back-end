using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Repository.Chat;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<int>> GetMessagesByIdsAsync(IEnumerable<int> messageIds, int chatId);
}