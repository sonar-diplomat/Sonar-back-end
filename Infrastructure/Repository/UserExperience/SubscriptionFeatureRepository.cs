using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class SubscriptionFeatureRepository : GenericRepository<Entities.Models.SubscriptionFeature>, ISubscriptionFeatureRepository
    {
        public SubscriptionFeatureRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
