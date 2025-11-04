using System.Text.Json;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.ClientSettings;
using Application.Response;
using Entities.Models.ClientSettings;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.ClientSettings;

public class ClientSettingsController(
    UserManager<User> userManager,
    ISettingsService settingsService
) : BaseController(userManager)
{
    // TODO: write XML comments and returnType attributes
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserSettings()
    {
        User user = await CheckAccessFeatures([]);
        Settings settings = await settingsService.GetByIdValidatedFullAsync(user.SettingsId);
        SettingsDTO dto = new SettingsDTO
        {
            Id = settings.Id,
            AutoPlay = settings.AutoPlay,
            Crossfade = settings.Crossfade,
            ExplicitContent = settings.ExplicitContent,
            PreferredPlaybackQuality = settings.PreferredPlaybackQuality != null ? new PlaybackQualityDTO
            {
                Id = settings.PreferredPlaybackQuality.Id,
                Name = settings.PreferredPlaybackQuality.Name,
                BitRate = settings.PreferredPlaybackQuality.BitRate,
                Description = settings.PreferredPlaybackQuality.Description
            } : null,
            Language = settings.Language != null ? new LanguageDTO
            {
                Id = settings.Language.Id,
                Locale = settings.Language.Locale,
                Name = settings.Language.Name,
                NativeName = settings.Language.NativeName
            } : null,
            Theme = settings.Theme != null ? new ThemeDTO
            {
                Id = settings.Theme.Id,
                Name = settings.Theme.Name,
                Description = settings.Theme.Description
            } : null,
            UserPrivacy = settings.UserPrivacy != null ? new UserPrivacySettingsDTO
            {
                Id = settings.UserPrivacy.Id,
                WhichCanViewProfile = settings.UserPrivacy.WhichCanViewProfile != null ? new UserPrivacyGroupDTO
                {
                    Id = settings.UserPrivacy.WhichCanViewProfile.Id,
                    Name = settings.UserPrivacy.WhichCanViewProfile.Name
                } : null,
                WhichCanMessage = settings.UserPrivacy.WhichCanMessage != null ? new UserPrivacyGroupDTO
                {
                    Id = settings.UserPrivacy.WhichCanMessage.Id,
                    Name = settings.UserPrivacy.WhichCanMessage.Name
                } : null
            } : null,
            NotificationTypes = settings.NotificationTypes?.Select(nt => new NotificationTypeDTO
            {
                Id = nt.Id,
                Name = nt.Name,
                Description = nt.Description
            }).ToList() ?? new List<NotificationTypeDTO>(),
            BlockedUserIds = settings.BlockedUsers?.Select(u => u.Id).ToList() ?? new List<int>()
        };
        throw ResponseFactory.Create<OkResponse<SettingsDTO>>(dto, ["User settings retrieved successfully."]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPatch]
    public async Task<IActionResult> PatchSettings([FromBody] JsonElement updates)
    {
        int settingsId = (await CheckAccessFeatures([])).SettingsId;
        Settings updated = await settingsService.PatchAsync(settingsId, updates);
        SettingsDTO dto = new SettingsDTO
        {
            Id = updated.Id,
            AutoPlay = updated.AutoPlay,
            Crossfade = updated.Crossfade,
            ExplicitContent = updated.ExplicitContent,
            PreferredPlaybackQuality = updated.PreferredPlaybackQuality != null ? new PlaybackQualityDTO
            {
                Id = updated.PreferredPlaybackQuality.Id,
                Name = updated.PreferredPlaybackQuality.Name,
                BitRate = updated.PreferredPlaybackQuality.BitRate,
                Description = updated.PreferredPlaybackQuality.Description
            } : null,
            Language = updated.Language != null ? new LanguageDTO
            {
                Id = updated.Language.Id,
                Locale = updated.Language.Locale,
                Name = updated.Language.Name,
                NativeName = updated.Language.NativeName
            } : null,
            Theme = updated.Theme != null ? new ThemeDTO
            {
                Id = updated.Theme.Id,
                Name = updated.Theme.Name,
                Description = updated.Theme.Description
            } : null,
            UserPrivacy = updated.UserPrivacy != null ? new UserPrivacySettingsDTO
            {
                Id = updated.UserPrivacy.Id,
                WhichCanViewProfile = updated.UserPrivacy.WhichCanViewProfile != null ? new UserPrivacyGroupDTO
                {
                    Id = updated.UserPrivacy.WhichCanViewProfile.Id,
                    Name = updated.UserPrivacy.WhichCanViewProfile.Name
                } : null,
                WhichCanMessage = updated.UserPrivacy.WhichCanMessage != null ? new UserPrivacyGroupDTO
                {
                    Id = updated.UserPrivacy.WhichCanMessage.Id,
                    Name = updated.UserPrivacy.WhichCanMessage.Name
                } : null
            } : null,
            NotificationTypes = updated.NotificationTypes?.Select(nt => new NotificationTypeDTO
            {
                Id = nt.Id,
                Name = nt.Name,
                Description = nt.Description
            }).ToList() ?? new List<NotificationTypeDTO>(),
            BlockedUserIds = updated.BlockedUsers?.Select(u => u.Id).ToList() ?? new List<int>()
        };
        throw ResponseFactory.Create<OkResponse<SettingsDTO>>(dto, ["Settings patched successfully"]);
    }
}