using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class UserPrivacySettingsRepository(SonarContext dbContext, IUserPrivacyGroupService userPrivacyGroupService) : GenericRepository<UserPrivacySettings>(dbContext), IUserPrivacySettingsRepository
    {
        public async Task<UserPrivacySettings> CreateDefaultAsync()
        {
            UserPrivacySettings userPrivacySettings = new UserPrivacySettings()
            {
                WhichCanMessage = await userPrivacyGroupService.GetDefaultAsync(),
                WhichCanViewProfile = await userPrivacyGroupService.GetDefaultAsync()
            };
            return await AddAsync(userPrivacySettings);
        }
    }
}
