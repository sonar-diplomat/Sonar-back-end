using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class GiftStyleService : IGiftStyleService
    {
        private readonly IGiftStyleRepository _repository;

        public GiftStyleService(IGiftStyleRepository repository)
        {
            _repository = repository;
        }

        public Task<GiftStyle> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<GiftStyle>> GetAllAsync() => throw new NotImplementedException();
        public Task<GiftStyle> CreateAsync(GiftStyle entity) => throw new NotImplementedException();
        public Task<GiftStyle> UpdateAsync(GiftStyle entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

