using Application.Abstractions.Interfaces.Repository.Chat;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Chat;

public class ChatRepository : GenericRepository<Entities.Models.Chat.Chat>, IChatRepository
{
    public ChatRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
