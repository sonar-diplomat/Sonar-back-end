using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class SubscriptionPackService : ISubscriptionPackService
    {
        private readonly ISubscriptionPackRepository _repository;

        public SubscriptionPackService(ISubscriptionPackRepository repository)
        {
            _repository = repository;
        }

        public Task<SubscriptionPack> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<SubscriptionPack>> GetAllAsync() => throw new NotImplementedException();
        public Task<SubscriptionPack> CreateAsync(SubscriptionPack entity) => throw new NotImplementedException();
        public Task<SubscriptionPack> UpdateAsync(SubscriptionPack entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

