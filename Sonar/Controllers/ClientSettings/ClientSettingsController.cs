using Application.Abstractions.Interfaces.Services;
using Application.DTOs.ClientSettings;
using Application.Response;
using Entities.Models.ClientSettings;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Sonar.Controllers.ClientSettings;

[Route("api/[controller]")]
[ApiController]
public class ClientSettingsController(
    UserManager<User> userManager,
    ISettingsService settingsService,
    ILanguageService languageService,
    IThemeService themeService,
    IPlaybackQualityService playbackQualityService,
    INotificationTypeService notificationTypeService
) : BaseController(userManager)
{
    /// <summary>
    /// Retrieves the current user's client settings including theme, language, privacy, and playback preferences.
    /// </summary>
    /// <returns>Settings DTO with full configuration details.</returns>
    /// <response code="200">User settings retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<SettingsDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUserSettings()
    {
        User user = await CheckAccessFeatures([]);
        Settings settings = await settingsService.GetByIdValidatedFullAsync(user.SettingsId);
        SettingsDTO dto = new()
        {
            Id = settings.Id,
            AutoPlay = settings.AutoPlay,
            Crossfade = settings.Crossfade,
            ExplicitContent = settings.ExplicitContent,
            PreferredPlaybackQuality = new PlaybackQualityDTO
            {
                Id = settings.PreferredPlaybackQuality.Id,
                Name = settings.PreferredPlaybackQuality.Name,
                BitRate = settings.PreferredPlaybackQuality.BitRate,
                Description = settings.PreferredPlaybackQuality.Description
            },
            Language = new LanguageDTO
            {
                Id = settings.Language.Id,
                Locale = settings.Language.Locale,
                Name = settings.Language.Name,
                NativeName = settings.Language.NativeName
            },
            Theme = new ThemeDTO
            {
                Id = settings.Theme.Id,
                Name = settings.Theme.Name,
                Description = settings.Theme.Description
            },
            UserPrivacy = new UserPrivacySettingsDTO
            {
                Id = settings.UserPrivacy.Id,
                WhichCanViewProfile = new UserPrivacyGroupDTO
                {
                    Id = settings.UserPrivacy.WhichCanViewProfile.Id,
                    Name = settings.UserPrivacy.WhichCanViewProfile.Name
                },
                WhichCanMessage = new UserPrivacyGroupDTO
                {
                    Id = settings.UserPrivacy.WhichCanMessage.Id,
                    Name = settings.UserPrivacy.WhichCanMessage.Name
                }
            },
            NotificationTypes = settings.NotificationTypes.Select(nt => new NotificationTypeDTO
            {
                Id = nt.Id,
                Name = nt.Name,
                Description = nt.Description
            }).ToList(),
            BlockedUserIds = settings.BlockedUsers.Select(u => u.Id).ToList()
        };
        throw ResponseFactory.Create<OkResponse<SettingsDTO>>(dto, ["User settings retrieved successfully."]);
    }

    /// <summary>
    /// Partially updates the current user's client settings using JSON Patch format.
    /// </summary>
    /// <param name="updates">JSON element containing the settings properties to update.</param>
    /// <returns>Updated settings DTO with all current values.</returns>
    /// <response code="200">Settings updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <remarks>
    /// Supports partial updates - only provide the fields you want to change.
    /// </remarks>
    [HttpPatch]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<SettingsDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PatchSettings([FromBody] JsonElement updates)
    {
        int settingsId = (await CheckAccessFeatures([])).SettingsId;
        Settings updated = await settingsService.PatchAsync(settingsId, updates);
        SettingsDTO dto = new()
        {
            Id = updated.Id,
            AutoPlay = updated.AutoPlay,
            Crossfade = updated.Crossfade,
            ExplicitContent = updated.ExplicitContent,
            PreferredPlaybackQuality = new PlaybackQualityDTO
            {
                Id = updated.PreferredPlaybackQuality.Id,
                Name = updated.PreferredPlaybackQuality.Name,
                BitRate = updated.PreferredPlaybackQuality.BitRate,
                Description = updated.PreferredPlaybackQuality.Description
            },
            Language = new LanguageDTO
            {
                Id = updated.Language.Id,
                Locale = updated.Language.Locale,
                Name = updated.Language.Name,
                NativeName = updated.Language.NativeName
            },
            Theme = new ThemeDTO
            {
                Id = updated.Theme.Id,
                Name = updated.Theme.Name,
                Description = updated.Theme.Description
            },
            UserPrivacy = new UserPrivacySettingsDTO
            {
                Id = updated.UserPrivacy.Id,
                WhichCanViewProfile = new UserPrivacyGroupDTO
                {
                    Id = updated.UserPrivacy.WhichCanViewProfile.Id,
                    Name = updated.UserPrivacy.WhichCanViewProfile.Name
                },
                WhichCanMessage = new UserPrivacyGroupDTO
                {
                    Id = updated.UserPrivacy.WhichCanMessage.Id,
                    Name = updated.UserPrivacy.WhichCanMessage.Name
                }
            },
            NotificationTypes = updated.NotificationTypes.Select(nt => new NotificationTypeDTO
            {
                Id = nt.Id,
                Name = nt.Name,
                Description = nt.Description
            }).ToList(),
            BlockedUserIds = updated.BlockedUsers.Select(u => u.Id).ToList()
        };
        throw ResponseFactory.Create<OkResponse<SettingsDTO>>(dto, ["Settings patched successfully"]);
    }

    /// <summary>
    /// Retrieves all available languages.
    /// </summary>
    /// <returns>List of all available languages with their details.</returns>
    /// <response code="200">Languages retrieved successfully.</response>
    [HttpGet("languages")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<LanguageDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLanguages()
    {
        IEnumerable<Language> languages = await languageService.GetAllAsync();
        IEnumerable<LanguageDTO> dto = languages.Select(l => new LanguageDTO
        {
            Id = l.Id,
            Locale = l.Locale,
            Name = l.Name,
            NativeName = l.NativeName
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<LanguageDTO>>>(dto, ["Languages retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves all available themes.
    /// </summary>
    /// <returns>List of all available themes with their details.</returns>
    /// <response code="200">Themes retrieved successfully.</response>
    [HttpGet("themes")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ThemeDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetThemes()
    {
        IEnumerable<Theme> themes = await themeService.GetAllAsync();
        IEnumerable<ThemeDTO> dto = themes.Select(t => new ThemeDTO
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<ThemeDTO>>>(dto, ["Themes retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves all available playback qualities.
    /// </summary>
    /// <returns>List of all available playback quality options with their details.</returns>
    /// <response code="200">Playback qualities retrieved successfully.</response>
    [HttpGet("playback-qualities")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<PlaybackQualityDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlaybackQualities()
    {
        IEnumerable<PlaybackQuality> playbackQualities = await playbackQualityService.GetAllAsync();
        IEnumerable<PlaybackQualityDTO> dto = playbackQualities.Select(pq => new PlaybackQualityDTO
        {
            Id = pq.Id,
            Name = pq.Name,
            BitRate = pq.BitRate,
            Description = pq.Description
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<PlaybackQualityDTO>>>(dto, ["Playback qualities retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves all available notification types.
    /// </summary>
    /// <returns>List of all available notification types with their details.</returns>
    /// <response code="200">Notification types retrieved successfully.</response>
    [HttpGet("notification-types")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<NotificationTypeDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotificationTypes()
    {
        IEnumerable<NotificationType> notificationTypes = await notificationTypeService.GetAllAsync();
        IEnumerable<NotificationTypeDTO> dto = notificationTypes.Select(nt => new NotificationTypeDTO
        {
            Id = nt.Id,
            Name = nt.Name,
            Description = nt.Description
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<NotificationTypeDTO>>>(dto, ["Notification types retrieved successfully"]);
    }
}