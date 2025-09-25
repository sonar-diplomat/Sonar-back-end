using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IGiftStyleService
    {
        Task<GiftStyle> GetByIdAsync(int id);
        Task<IEnumerable<GiftStyle>> GetAllAsync();
        Task<GiftStyle> CreateAsync(GiftStyle style);
        Task<GiftStyle> UpdateAsync(GiftStyle style);
        Task<bool> DeleteAsync(int id);
    }
}

