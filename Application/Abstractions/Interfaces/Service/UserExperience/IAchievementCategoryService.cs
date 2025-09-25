using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IAchievementCategoryService
    {
        Task<AchievementCategory> GetByIdAsync(int id);
        Task<IEnumerable<AchievementCategory>> GetAllAsync();
        Task<AchievementCategory> CreateAsync(AchievementCategory category);
        Task<AchievementCategory> UpdateAsync(AchievementCategory category);
        Task<bool> DeleteAsync(int id);
    }
}

