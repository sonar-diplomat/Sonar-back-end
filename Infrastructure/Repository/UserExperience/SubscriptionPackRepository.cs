using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class SubscriptionPackRepository : GenericRepository<Entities.Models.SubscriptionPack>, ISubscriptionPackRepository
    {
        public SubscriptionPackRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
