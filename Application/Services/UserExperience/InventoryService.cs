using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class InventoryService(IInventoryRepository repository) : IInventoryService
    {

        public Task<Inventory> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Inventory>> GetAllAsync() => throw new NotImplementedException();
        public Task<Inventory> CreateAsync(Inventory entity) => throw new NotImplementedException();
        public Task<Inventory> UpdateAsync(Inventory entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

