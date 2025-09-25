using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class AchievementProgressRepository : GenericRepository<Entities.Models.AchievementProgress>, IAchievementProgressRepository
    {
        public AchievementProgressRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
