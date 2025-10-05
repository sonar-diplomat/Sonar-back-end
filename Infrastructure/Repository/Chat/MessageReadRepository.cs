using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat;

public class MessageReadRepository : GenericRepository<MessageRead>, IMessageReadRepository
{
    public MessageReadRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}