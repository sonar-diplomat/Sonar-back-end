using Application.Abstractions.Interfaces.Repository.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class LicenseRepository : GenericRepository<Entities.Models.License>, ILicenseRepository
    {
        public LicenseRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
