using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
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

        public Task<Settings> CreateDefaultAsync()
        {
            return _repository.CreateDefaultAsync();
        }
        public Task<Settings> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Settings>> GetAllAsync() => throw new NotImplementedException();
        public Task<Settings> CreateAsync(Settings entity) => throw new NotImplementedException();
        public Task<Settings> UpdateAsync(Settings entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();


    }
}

