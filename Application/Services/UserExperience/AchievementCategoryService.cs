using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class AchievementCategoryService(IAchievementCategoryRepository repository) : IAchievementCategoryService
{
    public Task<AchievementCategory> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AchievementCategory>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AchievementCategory> CreateAsync(AchievementCategory entity)
    {
        throw new NotImplementedException();
    }

    public Task<AchievementCategory> UpdateAsync(AchievementCategory entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}