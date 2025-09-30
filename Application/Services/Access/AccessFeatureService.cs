using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Access;

namespace Application.Services.Access
{
    public class AccessFeatureService : IAccessFeatureService
    {
        private readonly IAccessFeatureRepository repository;

        public AccessFeatureService(IAccessFeatureRepository repository)
        {
            this.repository = repository;
        }

        public Task<AccessFeature> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AccessFeature>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AccessFeature> CreateAsync(AccessFeature entity)
        {
            throw new NotImplementedException();
        }

        public Task<AccessFeature> UpdateAsync(AccessFeature entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<AccessFeature>> GetDefaultAsync()
        {
            return repository.GetDefaultAsync();
        }
    }
}

