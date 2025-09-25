using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class DistributorRepository : GenericRepository<Distributor>, IDistributorRepository
    {
        public DistributorRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
