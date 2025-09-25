using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ICosmeticStickerService
    {
        Task<CosmeticSticker> GetByIdAsync(int id);
        Task<IEnumerable<CosmeticSticker>> GetAllAsync();
        Task<CosmeticSticker> CreateAsync(CosmeticSticker sticker);
        Task<CosmeticSticker> UpdateAsync(CosmeticSticker sticker);
        Task<bool> DeleteAsync(int id);
    }
}
