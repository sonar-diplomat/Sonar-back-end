using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class UserPrivacySettingsService : IUserPrivacySettingsService
    {
        private readonly IUserPrivacySettingsRepository _repository;

        public UserPrivacySettingsService(IUserPrivacySettingsRepository repository)
        {
            _repository = repository;
        }

        public Task<UserPrivacySettings> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<UserPrivacySettings>> GetAllAsync() => throw new NotImplementedException();
        public Task<UserPrivacySettings> CreateAsync(UserPrivacySettings entity) => throw new NotImplementedException();
        public Task<UserPrivacySettings> UpdateAsync(UserPrivacySettings entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

