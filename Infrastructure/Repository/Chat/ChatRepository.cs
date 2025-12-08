using Application.Abstractions.Interfaces.Repository.Chat;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Chat;

public class ChatRepository(SonarContext dbContext)
    : GenericRepository<Entities.Models.Chat.Chat>(dbContext), IChatRepository
{
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