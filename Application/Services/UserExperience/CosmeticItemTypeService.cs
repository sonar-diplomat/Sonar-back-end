using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class CosmeticItemTypeService(ICosmeticItemTypeRepository repository) : ICosmeticItemTypeService
    {

        public Task<CosmeticItemType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<CosmeticItemType>> GetAllAsync() => throw new NotImplementedException();
        public Task<CosmeticItemType> CreateAsync(CosmeticItemType entity) => throw new NotImplementedException();
        public Task<CosmeticItemType> UpdateAsync(CosmeticItemType entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

