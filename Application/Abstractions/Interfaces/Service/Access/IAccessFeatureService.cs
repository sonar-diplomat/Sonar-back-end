using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IAccessFeatureService
    {
        Task<AccessFeature> GetByIdAsync(int id);
        Task<IEnumerable<AccessFeature>> GetAllAsync();
        Task<ICollection<AccessFeature>> GetDefaultAsync();
        Task<AccessFeature> CreateAsync(AccessFeature feature);
        Task<AccessFeature> UpdateAsync(AccessFeature feature);
        Task<bool> DeleteAsync(int id);
    }
}