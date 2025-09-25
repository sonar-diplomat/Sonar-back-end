using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _repository;

        public SettingsService(ISettingsRepository repository)
        {
            _repository = repository;
        }

        public Task<Settings> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Settings>> GetAllAsync() => throw new NotImplementedException();
        public Task<Settings> CreateAsync(Settings entity) => throw new NotImplementedException();
        public Task<Settings> UpdateAsync(Settings entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

