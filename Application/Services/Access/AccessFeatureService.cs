using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Models.Access;

namespace Application.Services.Access;

public class AccessFeatureService(IAccessFeatureRepository repository)
    : GenericService<AccessFeature>(repository), IAccessFeatureService
{
    public async Task<ICollection<AccessFeature>> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }

    public async Task<ICollection<AccessFeature>> GetUserFeaturesByIdAsync(int userId)
    {
        return await repository.GetUserFeaturesByIdAsync(userId) ?? throw ResponseFactory.Create<NotFoundResponse>([$"User with id {userId} not found"]);
    }

    public async Task<AccessFeature> GetByNameValidatedAsync(string name)
    {
        return await repository.GetByNameValidatedAsync(name);
    }
}
