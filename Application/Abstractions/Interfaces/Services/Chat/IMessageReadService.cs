using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services;

public interface IMessageReadService : IGenericService<MessageRead>
{
    Task<IEnumerable<int>> GetReadMessageIdsAsync(int userId, List<int> validMessageIds);
    Task<MessageRead?> GetReadRecordAsync(int userId, int messageId);
    Task AddRangeAsync(IEnumerable<MessageRead> messageReads);
}