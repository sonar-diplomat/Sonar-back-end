using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Repository.Chat;

public interface IMessageReadRepository : IGenericRepository<MessageRead>
{
    Task<IEnumerable<int>> GetReadMessageIdsAsync(int userId, List<int> validMessageIds);
    Task<MessageRead?> GetAsync(int userId, int messageId);
    Task AddRangeAsync(IEnumerable<MessageRead> messageReads);
    Task<IEnumerable<MessageRead>> GetByMessageIdsAsync(List<int> messageIds);
    Task DeleteByMessageIdAsync(int messageId);
}