using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class SettingsService(
        ISettingsRepository repository,
        IPlaybackQualityService playbackQualityService,
        ILanguageService languageService,
        IThemeService themeService,
        IUserPrivacySettingsService userPrivacySettingsService) : GenericService<Settings>(repository), ISettingsService
    {
        public async Task<Settings> CreateDefaultAsync(string languageLocale)
        {

            Settings settings = new();
            settings.AutoPlay = true;
            settings.Crossfade = false;
            settings.ExplicitContent = false;
            settings.PreferredPlaybackQuality = await playbackQualityService.GetDefaultAsync();
            settings.Language = await languageService.GetByLocaleAsync(languageLocale);
            settings.Theme = await themeService.GetDefaultAsync();
            settings.UserPrivacy = await userPrivacySettingsService.CreateDefaultAsync();

            return settings;
        }
    }
}

