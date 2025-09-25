using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class AchievementRepository : GenericRepository<Entities.Models.Achievement>, IAchievementRepository
    {
        public AchievementRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
