using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Chat;

public class MessageRepository(
    SonarContext dbContext
) : GenericRepository<Message>(dbContext), IMessageRepository
{
    public async Task<IEnumerable<int>> GetMessagesByIdsAsync(IEnumerable<int> messageIds, int chatId)
    {
        return await context.Messages.AsQueryable()
            .Where(m => m.ChatId == chatId && messageIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync();
    }
}