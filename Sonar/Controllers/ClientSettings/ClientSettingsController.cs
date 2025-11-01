using System.Text.Json;
using Application.Abstractions.Interfaces.Services;
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
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserSettings()
    {
        User user = await CheckAccessFeatures([]);
        throw ResponseFactory.Create<OkResponse<Settings>>(settingsService.GetByIdValidatedFullAsync(user.SettingsId),
            ["User settings retrieved successfully."]);
    }

    [HttpPatch]
    public async Task<IActionResult> PatchSettings([FromBody] JsonElement updates)
    {
        int settingsId = (await CheckAccessFeatures([])).SettingsId;
        Settings updated = await settingsService.PatchAsync(settingsId, updates);
        throw ResponseFactory.Create<OkResponse<Settings>>(updated, ["Settings patched successfully"]);
    }
}