using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Repository.Access;

public interface IAccessFeatureRepository : IGenericRepository<AccessFeature>
{
    Task<ICollection<AccessFeature>> GetDefaultAsync();
    Task<ICollection<AccessFeature>> GetUserFeaturesByIdAsync(int userId);
    Task<AccessFeature> GetByNameValidatedAsync(string name);
}
