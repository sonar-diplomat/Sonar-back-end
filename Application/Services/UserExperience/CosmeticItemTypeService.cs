using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class CosmeticItemTypeService : ICosmeticItemTypeService
    {
        private readonly ICosmeticItemTypeRepository _repository;

        public CosmeticItemTypeService(ICosmeticItemTypeRepository repository)
        {
            _repository = repository;
        }

        public Task<CosmeticItemType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<CosmeticItemType>> GetAllAsync() => throw new NotImplementedException();
        public Task<CosmeticItemType> CreateAsync(CosmeticItemType entity) => throw new NotImplementedException();
        public Task<CosmeticItemType> UpdateAsync(CosmeticItemType entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

