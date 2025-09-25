using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IAchievementProgressService
    {
        Task<AchievementProgress> GetByIdAsync(int id);
        Task<IEnumerable<AchievementProgress>> GetAllAsync();
        Task<AchievementProgress> CreateAsync(AchievementProgress progress);
        Task<AchievementProgress> UpdateAsync(AchievementProgress progress);
        Task<bool> DeleteAsync(int id);
    }
}

