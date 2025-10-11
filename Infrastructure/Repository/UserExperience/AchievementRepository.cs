using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.UserExperience;

public class AchievementRepository : GenericRepository<Achievement>, IAchievementRepository
{
    public AchievementRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
