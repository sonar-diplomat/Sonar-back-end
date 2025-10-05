using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class DistributorService(IDistributorRepository repository) : IDistributorService
{
    public Task<Distributor> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Distributor>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Distributor> CreateAsync(Distributor entity)
    {
        throw new NotImplementedException();
    }

    public Task<Distributor> UpdateAsync(Distributor entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}