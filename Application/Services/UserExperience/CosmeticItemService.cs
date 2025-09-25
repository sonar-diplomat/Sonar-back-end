using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class CosmeticItemService : ICosmeticItemService
    {
        private readonly ICosmeticItemRepository _repository;

        public CosmeticItemService(ICosmeticItemRepository repository)
        {
            _repository = repository;
        }

        public Task<CosmeticItem> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<CosmeticItem>> GetAllAsync() => throw new NotImplementedException();
        public Task<CosmeticItem> CreateAsync(CosmeticItem entity) => throw new NotImplementedException();
        public Task<CosmeticItem> UpdateAsync(CosmeticItem entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

