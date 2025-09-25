using Application.Abstractions.Interfaces.Repository.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat
{
    public class ChatRepository : GenericRepository<Entities.Models.Chat>, IChatRepository
    {
        public ChatRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
