using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class SubscriptionFeatureService : ISubscriptionFeatureService
    {
        private readonly ISubscriptionFeatureRepository _repository;

        public SubscriptionFeatureService(ISubscriptionFeatureRepository repository)
        {
            _repository = repository;
        }

        public Task<SubscriptionFeature> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<SubscriptionFeature>> GetAllAsync() => throw new NotImplementedException();
        public Task<SubscriptionFeature> CreateAsync(SubscriptionFeature entity) => throw new NotImplementedException();
        public Task<SubscriptionFeature> UpdateAsync(SubscriptionFeature entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

