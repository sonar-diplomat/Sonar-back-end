using Application.Abstractions.Interfaces.Repository.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat
{
    public class MessageRepository : GenericRepository<Entities.Models.Message>, IMessageRepository
    {
        public MessageRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
