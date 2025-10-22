using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Repository.Distribution;

public interface ILicenseRepository : IGenericRepository<License>
{
    Task<License?> GetLicenseByKeyAsync(string licenseKey);
}
