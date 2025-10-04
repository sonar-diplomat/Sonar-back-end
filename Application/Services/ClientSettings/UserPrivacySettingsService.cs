using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class UserPrivacySettingsService(IUserPrivacySettingsRepository repository)
        : GenericService<UserPrivacySettings>(repository), IUserPrivacySettingsService
    {
        public async Task<UserPrivacySettings> GetDefaultAsync()
        {
            return await repository.CreateDefaultAsync();
        }
    }
}

