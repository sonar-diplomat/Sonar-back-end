using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Distribution;

public class DistributorAccountRepository(SonarContext dbContext)
    : GenericRepository<DistributorAccount>(dbContext), IDistributorAccountRepository
{
    public async Task<DistributorAccount?> GetByEmailAsync(string email)
    {
        return await context.DistributorAccounts.FirstOrDefaultAsync(da => da.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.DistributorAccounts.AnyAsync(da => da.Email == email);
    }
}
