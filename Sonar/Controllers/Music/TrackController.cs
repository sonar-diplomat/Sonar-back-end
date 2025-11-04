using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs.Music;
using Application.Response;
using Entities.Enums;
using Entities.Models.ClientSettings;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Sonar.Extensions;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class TrackController(
    UserManager<User> userManager,
    ITrackService trackService,
    ISettingsService settingsService,
    IShareService shareService) : ShareController<Track>(userManager, shareService)
{

    // TODO: write XML comments and returnType attributes
    [HttpGet("{trackId}/stream")]
    [Authorize]
    public async Task<IActionResult> StreamMusic(int trackId, [FromQuery] bool download = false)
    {
        int settingsId = (await CheckAccessFeatures([AccessFeatureStruct.ListenContent])).SettingsId;
        // TODO: Use settings to determine track quality
        Settings setttings = await settingsService.GetByIdValidatedAsync(settingsId);
        string? rangeHeader = Request.Headers.Range.FirstOrDefault();
        MusicStreamResultDTO? result = await trackService.GetMusicStreamAsync(trackId, rangeHeader);

        if (result == null) throw ResponseFactory.Create<NotFoundResponse>([$"Track with ID {trackId} not found"]);

        result.GetStreamDetails(out Stream stream, out string contentType, out bool enableRangeProcessing);

        if (download)
            // TODO: Validate User pack
            Response.Headers.Append("Content-Disposition", "attachment; filename=random-track.mp3");

        return File(stream, contentType, enableRangeProcessing);
    }

    // TODO: write XML comments and returnType attributes
    [HttpDelete("{trackId:int}")]
    public async Task<IActionResult> DeleteTrack(int trackId)
    {
        await this.CheckDistributorAsync();
        await trackService.DeleteAsync(trackId);
        throw ResponseFactory.Create<OkResponse>([$"Track with ID {trackId} successfully deleted"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("{trackId:int}")]
    public async Task<Track> UpdateTrackInfo(int trackId, [FromBody] UpdateTrackDTO dto)
    {
        await this.CheckDistributorAsync();
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        track.Title = dto.Title ?? track.Title;
        track.IsExplicit = dto.IsExplicit ?? track.IsExplicit;
        track.DrivingDisturbingNoises = dto.DrivingDisturbingNoises ?? track.DrivingDisturbingNoises;
        // TODO: create DTO
        track = await trackService.UpdateAsync(track);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track updated successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("{trackId:int}/audio-file")]
    public async Task UpdateTrackFile(int trackId, [FromForm] UpdateTrackFileDTO dto)
    {
        await this.CheckDistributorAsync();
        await trackService.UpdateTrackFileAsync(trackId, dto.PlaybackQualityId, dto.File);
        throw ResponseFactory.Create<OkResponse>(["Track audio file updated successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("{trackId:int}")]
    public async Task<IActionResult> GetTrackById(int trackId)
    {
        // TODO: create DTO
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track successfully retrieved"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("{trackId:int}/visibility")]
    public async Task<IActionResult> UpdateTrackVisibilityStatus(int trackId, [FromQuery] int visibilityStatusId)
    {
        await this.CheckDistributorAsync();
        await trackService.UpdateVisibilityStatusAsync(trackId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>(["Track visibility status was changed successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPost("{trackId:int}/toggle-favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavoriteTrack(int trackId)
    {
        User user = await CheckAccessFeatures([]);
        bool isFavorite = await trackService.ToggleFavoriteAsync(trackId, user.LibraryId);
        string message = isFavorite ? "Track added to favorites" : "Track removed from favorites";
        throw ResponseFactory.Create<OkResponse>([message]);
    }
}