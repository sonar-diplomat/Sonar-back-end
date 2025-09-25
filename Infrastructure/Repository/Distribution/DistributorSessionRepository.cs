using Application.Abstractions.Interfaces.Repository.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class DistributorSessionRepository : GenericRepository<Entities.Models.DistributorSession>, IDistributorSessionRepository
    {
        public DistributorSessionRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
