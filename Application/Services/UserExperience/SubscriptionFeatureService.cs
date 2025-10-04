using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class SubscriptionFeatureService(ISubscriptionFeatureRepository repository) : ISubscriptionFeatureService
    {

        public Task<SubscriptionFeature> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<SubscriptionFeature>> GetAllAsync() => throw new NotImplementedException();
        public Task<SubscriptionFeature> CreateAsync(SubscriptionFeature entity) => throw new NotImplementedException();
        public Task<SubscriptionFeature> UpdateAsync(SubscriptionFeature entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

