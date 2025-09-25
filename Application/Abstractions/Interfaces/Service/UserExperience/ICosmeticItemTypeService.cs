using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ICosmeticItemTypeService
    {
        Task<CosmeticItemType> GetByIdAsync(int id);
        Task<IEnumerable<CosmeticItemType>> GetAllAsync();
        Task<CosmeticItemType> CreateAsync(CosmeticItemType type);
        Task<CosmeticItemType> UpdateAsync(CosmeticItemType type);
        Task<bool> DeleteAsync(int id);
    }
}

