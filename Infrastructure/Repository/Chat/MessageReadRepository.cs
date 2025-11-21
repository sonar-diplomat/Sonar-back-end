using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Chat;

public class MessageReadRepository(SonarContext dbContext)
    : GenericRepository<MessageRead>(dbContext), IMessageReadRepository
{
    public async Task<IEnumerable<int>> GetReadMessageIdsAsync(int userId, List<int> validMessageIds)
    {
        return context.MessageReads
            .Where(mr => mr.UserId == userId && validMessageIds.Contains(mr.MessageId))
            .Select(mr => mr.MessageId);
    }

    public async Task<MessageRead?> GetAsync(int userId, int messageId)
    {
        return await context.MessageReads
            .FirstOrDefaultAsync(mr => mr.UserId == userId && mr.MessageId == messageId);
    }

    public async Task AddRangeAsync(IEnumerable<MessageRead> messageReads)
    {
        await context.MessageReads.AddRangeAsync(messageReads);
        await context.SaveChangesAsync();
    }
}