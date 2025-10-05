using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class AchievementProgressRepository : GenericRepository<AchievementProgress>, IAchievementProgressRepository
{
    public AchievementProgressRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}