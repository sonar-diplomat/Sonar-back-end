using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class AchievementService(IAchievementRepository repository) : IAchievementService
{
    public Task<Achievement> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Achievement>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Achievement> CreateAsync(Achievement entity)
    {
        throw new NotImplementedException();
    }

    public Task<Achievement> UpdateAsync(Achievement entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}