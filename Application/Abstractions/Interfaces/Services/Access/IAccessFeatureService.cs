using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Services;

public interface IAccessFeatureService : IGenericService<AccessFeature>
{
    Task<ICollection<AccessFeature>> GetDefaultAsync();
    Task<ICollection<AccessFeature>> GetUserFeaturesByIdAsync(int userId);
    Task<AccessFeature> GetByNameValidatedAsync(string name);
}
