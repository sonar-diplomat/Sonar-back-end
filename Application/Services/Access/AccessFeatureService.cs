using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models;
using Entities.Models.Access;

namespace Application.Services.Access
{
    public class AccessFeatureService : IAccessFeatureService
    {
        private readonly IAccessFeatureRepository _repository;

        public AccessFeatureService(IAccessFeatureRepository repository)
        {
            _repository = repository;
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
    }
}

