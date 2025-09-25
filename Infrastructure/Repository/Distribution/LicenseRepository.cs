using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class LicenseRepository : GenericRepository<License>, ILicenseRepository
    {
        public LicenseRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
