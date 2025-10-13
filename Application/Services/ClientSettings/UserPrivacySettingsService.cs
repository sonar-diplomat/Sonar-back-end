using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;
using Entities.Models.UserCore;

namespace Application.Services.ClientSettings;

public class UserPrivacySettingsService(
    IUserPrivacySettingsRepository repository,
    IUserPrivacyGroupService userPrivacyGroupService)
    : GenericService<UserPrivacySettings>(repository), IUserPrivacySettingsService
{
    public async Task<UserPrivacySettings> GetDefaultAsync()
    {
        UserPrivacyGroup userPrivacyGroup = await userPrivacyGroupService.GetDefaultAsync();

        UserPrivacySettings userPrivacySettings = new()
        {
            WhichCanMessage = userPrivacyGroup,
            WhichCanViewProfile = userPrivacyGroup
        };

        return await repository.AddAsync(userPrivacySettings);
    }
}
