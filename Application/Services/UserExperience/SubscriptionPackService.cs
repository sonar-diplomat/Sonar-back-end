using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class SubscriptionPackService(ISubscriptionPackRepository repository) : ISubscriptionPackService
{
    public Task<SubscriptionPack> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SubscriptionPack>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SubscriptionPack> CreateAsync(SubscriptionPack entity)
    {
        throw new NotImplementedException();
    }

    public Task<SubscriptionPack> UpdateAsync(SubscriptionPack entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}