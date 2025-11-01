using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.User;

public class QueueRepository(SonarContext dbContext) : GenericRepository<Queue>(dbContext), IQueueRepository
{
}