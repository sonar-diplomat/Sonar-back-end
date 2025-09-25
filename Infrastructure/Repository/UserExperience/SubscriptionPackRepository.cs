using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class SubscriptionPackRepository : GenericRepository<SubscriptionPack>, ISubscriptionPackRepository
    {
        public SubscriptionPackRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
