using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class AchievementCategoryRepository : GenericRepository<Entities.Models.AchievementCategory>, IAchievementCategoryRepository
    {
        public AchievementCategoryRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
