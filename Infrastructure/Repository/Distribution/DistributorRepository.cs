using Application.Abstractions.Interfaces.Repository.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class DistributorRepository : GenericRepository<Entities.Models.Distributor>, IDistributorRepository
    {
        public DistributorRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
