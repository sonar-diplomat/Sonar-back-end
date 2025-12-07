using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Application.Response;
using Entities.Models.ClientSettings;
using Entities.Models.UserCore;
using System.Text.Json;

namespace Application.Services.ClientSettings;

public class SettingsService(
    ISettingsRepository repository,
    IPlaybackQualityService playbackQualityService,
    ILanguageService languageService,
    IThemeService themeService,
    IUserPrivacySettingsService userPrivacySettingsService,
    IUserPrivacyGroupService userPrivacyGroupService,
    IUserRepository userRepository
) : GenericService<Settings>(repository), ISettingsService
{
    public async Task<Settings> GetByIdValidatedFullAsync(int userSettingsId)
    {
        return await repository
            .SnInclude(s => s.Language)
            .SnInclude(s => s.PreferredPlaybackQuality)
            .SnInclude(s => s.Theme)
            .SnInclude(s => s.UserPrivacy)
            .SnThenInclude(upg => upg.WhichCanViewProfile)
            .SnInclude(s => s.UserPrivacy)
            .SnThenInclude(upg => upg.WhichCanMessage)
            .SnInclude(s => s.NotificationTypes)
            .SnInclude(bu => bu.BlockedUsers)
            .GetByIdValidatedAsync(userSettingsId);
    }

    public async Task<Settings> CreateDefaultAsync(string languageLocale)
    {
        Settings settings = new()
        {
            AutoPlay = true,
            Crossfade = false,
            ExplicitContent = false,
            PreferredPlaybackQuality = await playbackQualityService.GetDefaultAsync(),
            Language = await languageService.GetByLocaleAsync(languageLocale),
            Theme = await themeService.GetDefaultAsync(),
            UserPrivacy = await userPrivacySettingsService.GetDefaultAsync()
        };
        return settings;
    }

    public async Task<Settings> PatchAsync(int settingsId, JsonElement updates)
    {
        Settings settings = await GetByIdValidatedFullAsync(settingsId);
        
        // Handle blockedUserIds separately (many-to-many relationship)
        JsonElement? blockedIdsJson = null;
        if (updates.TryGetProperty("blockedUserIds", out var blockedIdsElement))
        {
            blockedIdsJson = blockedIdsElement;
        }
        
        // Apply other patches normally (excluding blockedUserIds)
        settings.ApplyJsonPatch(updates);
        
        // Handle blockedUserIds after other patches
        if (blockedIdsJson.HasValue)
        {
            var blockedIds = blockedIdsJson.Value.Deserialize<List<int>>();
            if (blockedIds != null)
            {
                // Clear current blocked users
                settings.BlockedUsers.Clear();
                
                // Add new blocked users
                foreach (int userId in blockedIds)
                {
                    var user = await userRepository.GetByIdAsync(userId);
                    if (user == null)
                        throw ResponseFactory.Create<NotFoundResponse>([$"User with ID {userId} not found"]);
                    settings.BlockedUsers.Add(user);
                }
            }
        }
        
        await ValidateRelatedEntitiesAsync(settings);
        await repository.SaveChangesAsync();
        return settings;
    }

    private static async Task ValidateIdsAsync(IEnumerable<(int Id, Func<int, Task> Validate)> map)
    {
        foreach ((int id, Func<int, Task> validate) in map)
            if (id > 0)
                await validate(id);
    }

    private async Task ValidateRelatedEntitiesAsync(Settings settings)
    {
        await ValidateIdsAsync([
            (settings.LanguageId, languageService.GetByIdValidatedAsync),
            (settings.ThemeId, themeService.GetByIdValidatedAsync),
            (settings.PreferredPlaybackQualityId, playbackQualityService.GetByIdValidatedAsync),
            (settings.UserPrivacySettingsId, userPrivacySettingsService.GetByIdValidatedAsync)
        ]);
        await ValidateIdsAsync([
            (settings.UserPrivacy.WhichCanMessageId, userPrivacyGroupService.GetByIdValidatedAsync),
            (settings.UserPrivacy.WhichCanViewProfileId, userPrivacyGroupService.GetByIdValidatedAsync)
        ]);
    }
}