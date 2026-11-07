using Application.Abstractions.Interfaces.Repository.Chat;
using System.Data;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Chat;

public class ChatRepository(SonarContext dbContext)
    : GenericRepository<Entities.Models.Chat.Chat>(dbContext), IChatRepository
{
    public virtual async Task<TResult> ExecuteLockedAsync<TResult>(int chatId, Func<Task<TResult>> operation)
    {
        if (dbContext.ChangeTracker.HasChanges() || dbContext.Database.CurrentTransaction != null)
            throw new InvalidOperationException("Chat mutations require a clean unit of work and their own transaction.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        try
        {
            // PostgreSQL row locks serialize independent contexts and replicas. Read
            // membership only after acquiring the lock, including on a reused context.
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT 1 FROM \"Chat\" WHERE \"Id\" = {chatId} FOR UPDATE");
            dbContext.ChangeTracker.Clear();
            var result = await operation();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            // Saved entities may no longer represent the rolled-back database state.
            dbContext.ChangeTracker.Clear();
            throw;
        }
    }

    public async Task<Entities.Models.Chat.Chat?> FindPersonalChatBetweenUsersAsync(int userId1, int userId2)
    {
        return await dbContext.Set<Entities.Models.Chat.Chat>()
            .Include(c => c.Members)
            .Where(c => !c.IsGroup)
            .Where(c => c.Members.Count == 2)
            .Where(c => c.Members.Any(m => m.Id == userId1) && c.Members.Any(m => m.Id == userId2))
            .FirstOrDefaultAsync();
    }
}
