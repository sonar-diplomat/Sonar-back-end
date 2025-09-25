using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IAchievementService
    {
        Task<Achievement> GetByIdAsync(int id);
        Task<IEnumerable<Achievement>> GetAllAsync();
        Task<Achievement> CreateAsync(Achievement achievement);
        Task<Achievement> UpdateAsync(Achievement achievement);
        Task<bool> DeleteAsync(int id);
    }
}

