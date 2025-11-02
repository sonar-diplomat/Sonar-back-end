using System.IdentityModel.Tokens.Jwt;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs.Music;
using Application.Response;
using Entities.Enums;
using Entities.Models.ClientSettings;
using Entities.Models.Distribution;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class TrackController(
    UserManager<User> userManager,
    IDistributorAccountService accountService,
    IDistributorService distributorService,
    ITrackService trackService,
    ISettingsService settingsService,
    IShareService shareService) : ShareController<Track>(userManager, shareService)
{
    [Authorize]
    private async Task<DistributorAccount> GetDistributorAccountByJwt()
    {
        string? email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst("email")?.Value;
        if (email == null)
            throw ResponseFactory.Create<UnauthorizedResponse>(["Invalid JWT token"]);
        DistributorAccount? distributorAccount = await accountService.GetByEmailAsync(email);
        return distributorAccount ?? throw ResponseFactory.Create<UnauthorizedResponse>();
    }

    [Authorize]
    private async Task<Distributor> CheckDistributor()
    {
        DistributorAccount distributorAccount = await GetDistributorAccountByJwt();
        if (!Request.Headers.TryGetValue("X-Api-Key", out StringValues apiKey))
            throw ResponseFactory.Create<UnauthorizedResponse>();
        string key = apiKey.ToString();
        if (string.IsNullOrEmpty(key))
            throw ResponseFactory.Create<UnauthorizedResponse>();
        Distributor? distributor = await distributorService.GetByApiKeyAsync(key);
        return !(await accountService.GetAllByDistributor(distributor)).Contains(distributorAccount)
            ? throw ResponseFactory.Create<UnauthorizedResponse>()
            : distributor!;
    }

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

    [HttpDelete("{trackId:int}")]
    public async Task<IActionResult> DeleteTrack(int trackId)
    {
        await CheckDistributor();
        await trackService.DeleteAsync(trackId);
        throw ResponseFactory.Create<OkResponse>([$"Track with ID {trackId} successfully deleted"]);
    }

    [HttpPut("{trackId:int}")]
    public async Task<Track> UpdateTrackInfo(int trackId, UpdateTrackDTO dto)
    {
        await CheckDistributor();
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        track.Title = dto.Title ?? track.Title;
        track.IsExplicit = dto.IsExplicit ?? track.IsExplicit;
        track.DrivingDisturbingNoises = dto.DrivingDisturbingNoises ?? track.DrivingDisturbingNoises;
        track = await trackService.UpdateAsync(track);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track updated successfully"]);
    }

    [HttpPut("{trackId:int}/audio-file")]
    public async Task UpdateTrackFile(int trackId, UpdateTrackFileDTO dto)
    {
        await CheckDistributor();
        await trackService.UpdateTrackFileAsync(trackId, dto.PlaybackQualityId, dto.File);
        throw ResponseFactory.Create<OkResponse>(["Track audio file updated successfully"]);
    }

    [HttpGet("{trackId:int}")]
    public async Task<IActionResult> GetTrackById(int trackId)
    {
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track successfully retrieved"]);
    }

    [HttpPut("{trackId:int}/visibility")]
    public async Task<IActionResult> UpdateTrackVisibilityStatus(int trackId, int visibilityStatusId)
    {
        await CheckDistributor();
        await trackService.UpdateVisibilityStatusAsync(trackId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>(["Track visibility status was changed successfully"]);
    }

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