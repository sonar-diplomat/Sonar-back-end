using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Distribution;

public class DistributorRepository(SonarContext dbContext)
    : GenericRepository<Distributor>(dbContext), IDistributorRepository
{
    public async Task<Distributor?> GetByApiKeyAsync(string licenseKey)
    {
        return await context.Distributors.FirstOrDefaultAsync(d => d.License.LicenseKey == licenseKey);
    }
}
