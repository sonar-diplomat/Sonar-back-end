using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Response;
using Entities.Models.ClientSettings;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class TrackController(
    UserManager<User> userManager,
    IDistributorAccountService accountService,
    IDistributorService distributorService,
    ITrackService trackService,
    ISettingsService settingsService) : BaseControllerExtended(userManager, accountService, distributorService)
{
    [HttpGet("stream/{trackId:int}")]
    [Authorize]
    public async Task<IActionResult> StreamMusic(int trackId, [FromQuery] bool download = false)
    {
        int settingsId = (await CheckAccessFeatures([])).SettingsId;
        Settings settings = await settingsService.GetByIdValidatedAsync(settingsId);
        string? rangeHeader = Request.Headers.Range.FirstOrDefault();
        MusicStreamResultDTO? result = await trackService.GetMusicStreamAsync(trackId, rangeHeader);

        if (result == null) throw ResponseFactory.Create<NotFoundResponse>([$"Track with ID {trackId} not found"]);

        result.GetStreamDetails(out Stream stream, out string contentType, out bool enableRangeProcessing);

        if (download)
            // TODO: Validate User pack
            Response.Headers.Append("Content-Disposition", "attachment; filename=random-track.mp3");

        return File(stream, contentType, enableRangeProcessing);
    }

    [HttpDelete("{trackId:int}")]
    public async Task<IActionResult> DeleteTrack(int trackId)
    {
        await CheckDistributor();
        await trackService.DeleteAsync(trackId);
        throw ResponseFactory.Create<OkResponse>([$"Track with ID {trackId} successfully deleted"]);
    }

    [HttpPut("{trackId:int}")]
    public async Task<Track> UpdateTrack(int trackId, UpdateTrackDTO dto)
    {
        await CheckDistributor();
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        track.Title = dto.Title ?? track.Title;
        track.IsExplicit = dto.IsExplicit ?? track.IsExplicit;
        track.DrivingDisturbingNoises = dto.DrivingDisturbingNoises ?? track.DrivingDisturbingNoises;
        track = await trackService.UpdateAsync(track);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track updated successfully"]);
    }

    [HttpGet("{trackId:int}")]
    public async Task<IActionResult> GetTrackById(int trackId)
    {
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track successfully retrieved"]);
    }
}