using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class DistributorSessionService(IDistributorSessionRepository repository) : IDistributorSessionService
{
    public Task<DistributorSession> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DistributorSession>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<DistributorSession> CreateAsync(DistributorSession entity)
    {
        throw new NotImplementedException();
    }

    public Task<DistributorSession> UpdateAsync(DistributorSession entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}