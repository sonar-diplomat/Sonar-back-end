using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Services;

public interface IAccessFeatureService : IGenericService<AccessFeature>
{
    Task<ICollection<AccessFeature>> GetDefaultAsync();
    Task<ICollection<AccessFeature>> GetUserFeaturesByIdAsync(int userId);
    Task AssignAccessFeaturesAsync(int userId, int[] accessFeatureIds);
    Task RevokeAccessFeaturesAsync(int userId, int[] accessFeatureIds);
}
