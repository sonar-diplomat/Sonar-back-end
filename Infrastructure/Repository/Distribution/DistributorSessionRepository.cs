using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class DistributorSessionRepository : GenericRepository<DistributorSession>, IDistributorSessionRepository
    {
        public DistributorSessionRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
