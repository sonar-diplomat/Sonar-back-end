using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings;

public class SettingsService(
    ISettingsRepository repository,
    IPlaybackQualityService playbackQualityService,
    ILanguageService languageService,
    IThemeService themeService,
    IUserPrivacySettingsService userPrivacySettingsService) : GenericService<Settings>(repository), ISettingsService
{
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
}