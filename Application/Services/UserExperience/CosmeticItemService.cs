using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class CosmeticItemService(ICosmeticItemRepository repository) : ICosmeticItemService
    {

        public Task<CosmeticItem> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<CosmeticItem>> GetAllAsync() => throw new NotImplementedException();
        public Task<CosmeticItem> CreateAsync(CosmeticItem entity) => throw new NotImplementedException();
        public Task<CosmeticItem> UpdateAsync(CosmeticItem entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

