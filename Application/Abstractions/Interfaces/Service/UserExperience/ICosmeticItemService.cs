using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ICosmeticItemService
    {
        Task<CosmeticItem> GetByIdAsync(int id);
        Task<IEnumerable<CosmeticItem>> GetAllAsync();
        Task<CosmeticItem> CreateAsync(CosmeticItem item);
        Task<CosmeticItem> UpdateAsync(CosmeticItem item);
        Task<bool> DeleteAsync(int id);
    }
}