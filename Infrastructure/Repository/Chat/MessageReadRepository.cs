using Application.Abstractions.Interfaces.Repository.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat
{
    public class MessageReadRepository : GenericRepository<Entities.Models.MessageRead>, IMessageReadRepository
    {
        public MessageReadRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
