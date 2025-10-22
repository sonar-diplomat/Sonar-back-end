using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Enums;
using Entities.Models.ClientSettings;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class TrackController(UserManager<User> userManager, ITrackService trackService, ISettingsService settingsService) : BaseController(userManager)
{
    [HttpGet("stream")]
    [Authorize]
    public async Task<IActionResult> StreamMusic(int trackId)
    {
        int settingsId = (await CheckAccessFeatures([])).SettingsId;
        Settings setttings = await settingsService.GetByIdValidatedAsync(settingsId);
        string? rangeHeader = Request.Headers.Range.FirstOrDefault();
        MusicStreamResultDTO? result = await trackService.GetMusicStreamAsync(trackId, rangeHeader);

        if (result == null)
        {
            throw ResponseFactory.Create<NotFoundResponse>([$"Track with ID {trackId} not found"]);
        }

        result.GetStreamDetails(out Stream stream, out string contentType, out bool enableRangeProcessing);

        return File(stream, contentType, enableRangeProcessing: enableRangeProcessing);
    }

    [HttpDelete("{trackId}")]
    public async Task<IActionResult> DeleteTrack(int trackId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
    }
    
    [HttpPut("{trackId}")]
    public async Task<IActionResult> UpdateTrack(int trackId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{trackId}")]
    public async Task<IActionResult> GetTrackById(int trackId)
    {
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        throw ResponseFactory.Create<OkResponse<Track>>(track, [$"Track successfully retrieved"]);
    }

    [HttpGet("{trackId}/download")]
    public async Task<IActionResult> DownloadTrack(int trackId)
    {
        throw new NotImplementedException();
    }
}
