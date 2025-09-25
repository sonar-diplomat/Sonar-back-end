using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Access
{
    public class AccessFeatureRepository : GenericRepository<AccessFeature>, IAccessFeatureRepository
    {
        public AccessFeatureRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
