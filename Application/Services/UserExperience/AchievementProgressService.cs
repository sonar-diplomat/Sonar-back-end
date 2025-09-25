using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class AchievementProgressService : IAchievementProgressService
    {
        private readonly IAchievementProgressRepository _repository;

        public AchievementProgressService(IAchievementProgressRepository repository)
        {
            _repository = repository;
        }

        public Task<AchievementProgress> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<AchievementProgress>> GetAllAsync() => throw new NotImplementedException();
        public Task<AchievementProgress> CreateAsync(AchievementProgress entity) => throw new NotImplementedException();
        public Task<AchievementProgress> UpdateAsync(AchievementProgress entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

