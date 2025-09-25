using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _repository;

        public AchievementService(IAchievementRepository repository)
        {
            _repository = repository;
        }

        public Task<Achievement> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Achievement>> GetAllAsync() => throw new NotImplementedException();
        public Task<Achievement> CreateAsync(Achievement entity) => throw new NotImplementedException();
        public Task<Achievement> UpdateAsync(Achievement entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

