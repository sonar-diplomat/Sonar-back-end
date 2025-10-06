using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserPrivacySettingsService : IGenericService<UserPrivacySettings>
{
    Task<UserPrivacySettings> GetDefaultAsync();
}