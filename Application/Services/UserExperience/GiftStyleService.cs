using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class GiftStyleService(IGiftStyleRepository repository) : IGiftStyleService
{
    public Task<GiftStyle> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GiftStyle>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<GiftStyle> CreateAsync(GiftStyle entity)
    {
        throw new NotImplementedException();
    }

    public Task<GiftStyle> UpdateAsync(GiftStyle entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}