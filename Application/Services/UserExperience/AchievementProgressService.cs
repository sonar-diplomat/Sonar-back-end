using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class AchievementProgressService(IAchievementProgressRepository repository) : IAchievementProgressService
    {

        public Task<AchievementProgress> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<AchievementProgress>> GetAllAsync() => throw new NotImplementedException();
        public Task<AchievementProgress> CreateAsync(AchievementProgress entity) => throw new NotImplementedException();
        public Task<AchievementProgress> UpdateAsync(AchievementProgress entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

