using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Chat;
using Entities.Models.Chat;

namespace Application.Services.Chat;

public class MessageReadService(IMessageReadRepository repository)
    : GenericService<MessageRead>(repository), IMessageReadService
{
    public Task<IEnumerable<int>> GetReadMessageIdsAsync(int userId, List<int> validMessageIds)
    {
        return repository.GetReadMessageIdsAsync(userId, validMessageIds);
    }

    public Task<MessageRead?> GetReadRecordAsync(int userId, int messageId)
    {
        return repository.GetAsync(userId, messageId);
    }

    public Task AddRangeAsync(IEnumerable<MessageRead> messageReads)
    {
        return repository.AddRangeAsync(messageReads);
    }

    public async Task<IEnumerable<MessageReadDTO>> GetReadRecordsByMessageIdsAsync(List<int> messageIds)
    {
        var messageReads = await repository.GetByMessageIdsAsync(messageIds);
        return messageReads.Select(mr => new MessageReadDTO
        {
            ReadAt = mr.ReadAt,
            MessageId = mr.MessageId,
            UserId = mr.UserId
        });
    }
}