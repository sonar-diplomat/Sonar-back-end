using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class AchievementCategoryService : IAchievementCategoryService
    {
        private readonly IAchievementCategoryRepository _repository;

        public AchievementCategoryService(IAchievementCategoryRepository repository)
        {
            _repository = repository;
        }

        public Task<AchievementCategory> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<AchievementCategory>> GetAllAsync() => throw new NotImplementedException();
        public Task<AchievementCategory> CreateAsync(AchievementCategory entity) => throw new NotImplementedException();
        public Task<AchievementCategory> UpdateAsync(AchievementCategory entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

