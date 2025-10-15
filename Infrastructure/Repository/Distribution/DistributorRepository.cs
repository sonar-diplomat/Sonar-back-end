using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Distribution;

public class DistributorRepository : GenericRepository<Distributor>, IDistributorRepository
{
    public DistributorRepository(SonarContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<Distributor?> GetByApiKeyAsync(string licenseKey)
    {
        return await context.Distributors.FirstOrDefaultAsync(d => d.License.LicenseKey == licenseKey);
    }
}
