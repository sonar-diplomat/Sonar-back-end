using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class SubscriptionFeatureRepository : GenericRepository<SubscriptionFeature>, ISubscriptionFeatureRepository
    {
        public SubscriptionFeatureRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
