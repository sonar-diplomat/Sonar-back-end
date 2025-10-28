using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Access;
using Entities.Models.UserCore;

namespace Application.Services.Access;

public class AccessFeatureService(IAccessFeatureRepository repository, IUserService userService)
    : GenericService<AccessFeature>(repository), IAccessFeatureService
{
    public async Task<ICollection<AccessFeature>> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }

    public async Task<ICollection<AccessFeature>> GetUserFeaturesByIdAsync(int userId)
    {
        return await repository.GetUserFeaturesByIdAsync(userId);
    }

    public async Task AssignAccessFeaturesAsync(int userId, int[] accessFeatureIds)
    {
        User user = await userService.GetValidatedIncludeAccessFeaturesAsync(userId);
        foreach (int accessFeatureId in accessFeatureIds)
        {
            if (user.AccessFeatures.All(af => af.Id != accessFeatureId))
                user.AccessFeatures.Add(await GetByIdValidatedAsync(accessFeatureId));
        }
        await repository.SaveChangesAsync();
    }

    public async Task AssignAccessFeaturesByNameAsync(int userId, string[] accessFeatures)
    {
        User user = await userService.GetValidatedIncludeAccessFeaturesAsync(userId);
        foreach (string name in accessFeatures)
        {
            if (user.AccessFeatures.All(af => af.Name != name))
                user.AccessFeatures.Add(await repository.GetByNameValidatedAsync(name));
        }
        await repository.SaveChangesAsync();
    }

    public async Task RevokeAccessFeaturesAsync(int userId, int[] accessFeatureIds)
    {
        var user = await userService.GetValidatedIncludeAccessFeaturesAsync(userId);
        IEnumerable<AccessFeature> toRemove = user.AccessFeatures.Where(af => accessFeatureIds.Contains(af.Id));
        foreach (AccessFeature af in toRemove)
            user.AccessFeatures.Remove(af);
        await repository.SaveChangesAsync();
    }

    public async Task RevokeAccessFeaturesByNameAsync(int userId, string[] accessFeatures)
    {
        var user = await userService.GetValidatedIncludeAccessFeaturesAsync(userId);
        IEnumerable<AccessFeature> toRemove = user.AccessFeatures.Where(af => accessFeatures.Contains(af.Name));
        foreach (AccessFeature af in toRemove)
            user.AccessFeatures.Remove(af);
        await repository.SaveChangesAsync();
    }
}
