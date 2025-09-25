using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _repository;

        public GiftService(IGiftRepository repository)
        {
            _repository = repository;
        }

        public Task<Gift> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Gift>> GetAllAsync() => throw new NotImplementedException();
        public Task<Gift> CreateAsync(Gift entity) => throw new NotImplementedException();
        public Task<Gift> UpdateAsync(Gift entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

