using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Repository.Distribution;

public interface IDistributorRepository : IGenericRepository<Distributor>
{
    public Task<Distributor?> GetByApiKeyAsync(string licenseKey);
}
