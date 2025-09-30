using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ISettingsService
    {
        Task<Settings> GetByIdAsync(int id);
        Task<Settings> CreateDefaultAsync();
        Task<IEnumerable<Settings>> GetAllAsync();
        Task<Settings> CreateAsync(Settings settings);
        Task<Settings> UpdateAsync(Settings settings);
        Task<bool> DeleteAsync(int id);
    }
}

