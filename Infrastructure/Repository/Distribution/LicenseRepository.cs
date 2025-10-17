using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Distribution;

public class LicenseRepository(SonarContext dbContext) : GenericRepository<License>(dbContext), ILicenseRepository
{
    public async Task<License?>GetLicenseByKeyAsync(string licenseKey)
    {
        return await context.Licenses.FirstOrDefaultAsync(l => l.LicenseKey == licenseKey);
    }
}
