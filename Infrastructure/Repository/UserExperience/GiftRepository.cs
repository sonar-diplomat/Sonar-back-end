using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class GiftRepository : GenericRepository<Entities.Models.Gift>, IGiftRepository
    {
        public GiftRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
