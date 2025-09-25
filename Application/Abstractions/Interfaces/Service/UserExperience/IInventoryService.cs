using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IInventoryService
    {
        Task<Inventory> GetByIdAsync(int id);
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory> CreateAsync(Inventory inventory);
        Task<Inventory> UpdateAsync(Inventory inventory);
        Task<bool> DeleteAsync(int id);
    }
}