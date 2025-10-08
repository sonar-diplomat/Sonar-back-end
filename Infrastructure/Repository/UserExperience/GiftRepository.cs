using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class GiftRepository : GenericRepository<Gift>, IGiftRepository
{
    public GiftRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
