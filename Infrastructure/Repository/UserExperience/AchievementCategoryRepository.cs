using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class AchievementCategoryRepository : GenericRepository<AchievementCategory>, IAchievementCategoryRepository
{
    public AchievementCategoryRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}