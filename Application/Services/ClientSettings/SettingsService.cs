using System.Text.Json;
using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings;

public class SettingsService(
    ISettingsRepository repository,
    IPlaybackQualityService playbackQualityService,
    ILanguageService languageService,
    IThemeService themeService,
    IUserPrivacySettingsService userPrivacySettingsService,
    IUserPrivacyGroupService userPrivacyGroupService
) : GenericService<Settings>(repository), ISettingsService
{
    public async Task<Settings> GetByIdValidatedFullAsync(int userSettingsId)
    {
        return await repository
            .Include(s => s.Language)
            .Include(s => s.PreferredPlaybackQuality)
            .Include(s => s.Theme)
            .Include(s => s.UserPrivacy)
            .ThenInclude(upg => upg.WhichCanViewProfile)
            .Include(s => s.UserPrivacy)
            .ThenInclude(upg => upg.WhichCanMessage)
            .Include(s => s.NotificationTypes)
            .Include(bu => bu.BlockedUsers)
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
        settings.ApplyJsonPatch(updates);
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