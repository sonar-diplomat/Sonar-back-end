using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}