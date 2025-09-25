using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IGiftService
    {
        Task<Gift> GetByIdAsync(int id);
        Task<IEnumerable<Gift>> GetAllAsync();
        Task<Gift> CreateAsync(Gift gift);
        Task<Gift> UpdateAsync(Gift gift);
        Task<bool> DeleteAsync(int id);
    }
}

