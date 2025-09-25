using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class CosmeticStickerService : ICosmeticStickerService
    {
        private readonly ICosmeticStickerRepository _repository;

        public CosmeticStickerService(ICosmeticStickerRepository repository)
        {
            _repository = repository;
        }

        public Task<CosmeticSticker> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<CosmeticSticker>> GetAllAsync() => throw new NotImplementedException();
        public Task<CosmeticSticker> CreateAsync(CosmeticSticker entity) => throw new NotImplementedException();
        public Task<CosmeticSticker> UpdateAsync(CosmeticSticker entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

