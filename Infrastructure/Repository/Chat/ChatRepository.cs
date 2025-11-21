using Application.Abstractions.Interfaces.Repository.Chat;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Chat;

public class ChatRepository(SonarContext dbContext)
    : GenericRepository<Entities.Models.Chat.Chat>(dbContext), IChatRepository
{
}