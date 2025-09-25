using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IUserPrivacySettingsService
    {
        Task<UserPrivacySettings> GetByIdAsync(int id);
        Task<IEnumerable<UserPrivacySettings>> GetAllAsync();
        Task<UserPrivacySettings> CreateAsync(UserPrivacySettings settings);
        Task<UserPrivacySettings> UpdateAsync(UserPrivacySettings settings);
        Task<bool> DeleteAsync(int id);
    }
}

