using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
using Application.DTOs.Music;
using Application.Response;
using Entities.Enums;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class PlaylistController(
    UserManager<User> userManager,
    IPlaylistService playlistService,
    ICollectionService<Playlist> collectionService,
    IShareService shareService)
    : CollectionController<Playlist>(userManager, collectionService)
{
    // TODO: write XML comments and returnType attributes
    [HttpPost("create")]
    public async Task<IActionResult> CreatePlaylist([FromForm] CreatePlaylistDTO dto)
    {
        User user = await CheckAccessFeatures([]);
        Playlist playlist = await playlistService.CreatePlaylistAsync(user.Id, dto);
        PlaylistResponseDTO responseDto = new()
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CoverUrl = playlist.Cover?.Url ?? string.Empty,
            CreatorName = playlist.Creator?.UserName ?? string.Empty,
            TrackCount = playlist.Tracks?.Count ?? 0,
            ContributorNames = playlist.Contributors?.Select(c => c.UserName).ToList() ?? new List<string>()
        };
        throw ResponseFactory.Create<OkResponse<PlaylistResponseDTO>>(responseDto, ["Playlist was created successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpDelete("{playlistId:int}")]
    [Authorize]
    public async Task<IActionResult> DeletePlaylist(int playlistId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.DeleteAsync(playlistId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist was deleted successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("{playlistId:int}/update-name")]
    public async Task<IActionResult> UpdatePlaylistName(int playlistId, [FromQuery] string newName)
    {
        User user = await CheckAccessFeatures([]);
        Playlist playlist = await playlistService.UpdateNameAsync(playlistId, user.Id, newName);
        PlaylistResponseDTO responseDto = new()
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CoverUrl = playlist.Cover?.Url ?? string.Empty,
            CreatorName = playlist.Creator?.UserName ?? string.Empty,
            TrackCount = playlist.Tracks?.Count ?? 0,
            ContributorNames = playlist.Contributors?.Select(c => c.UserName).ToList() ?? new List<string>()
        };
        throw ResponseFactory.Create<OkResponse<PlaylistResponseDTO>>(responseDto, ["Playlist name was updated successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("{playlistId:int}/update-cover")]
    public async Task<IActionResult> UpdatePlaylistCover(int playlistId, [FromForm] IFormFile coverFile)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.UpdatePlaylistCoverAsync(playlistId, user.Id, coverFile);
        throw ResponseFactory.Create<OkResponse>(["Playlist cover was updated successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPost("{playlistId:int}/add-contributor/{contributorId:int}")]
    [Authorize]
    public async Task<IActionResult> AddContributor(int playlistId, int contributorId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.AddContributorAsync(playlistId, contributorId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist contributor was added successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpDelete("{playlistId:int}/delete-contributor/{contributorId:int}")]
    [Authorize]
    public async Task<IActionResult> RemoveContributor(int playlistId, int contributorId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.RemoveContributorAsync(playlistId, contributorId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist contributor was removed successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPost("{playlistId:int}/add-track/{trackId:int}")]
    [Authorize]
    public async Task<IActionResult> AddTrack(int playlistId, int trackId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.AddTrackToPlaylistAsync(playlistId, trackId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Track was added to playlist successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpDelete("{playlistId:int}/remove-track/{trackId:int}")]
    public async Task<IActionResult> RemoveTrack(int playlistId, int trackId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.RemoveTrackFromPlaylistAsync(playlistId, trackId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Track was removed from playlist successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("{playlistId}/tracks")]
    public async Task<IActionResult> GetPlaylistWithCursor(int playlistId, [FromQuery] string? after,
        [FromQuery] int limit = 20)
    {
        CursorPageDTO<TrackDTO> result = await playlistService.GetPlaylistTracksAsync(playlistId, after, limit);
        throw ResponseFactory.Create<OkResponse<CursorPageDTO<TrackDTO>>>(result,
            ["Playlist tracks retrieved successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("{playlistId:int}")]
    public async Task<IActionResult> GetPlaylistById(int playlistId)
    {
        Playlist playlist = await playlistService.GetByIdValidatedAsync(playlistId);
        PlaylistResponseDTO responseDto = new()
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CoverUrl = playlist.Cover?.Url ?? string.Empty,
            CreatorName = playlist.Creator?.UserName ?? string.Empty,
            TrackCount = playlist.Tracks?.Count ?? 0,
            ContributorNames = playlist.Contributors?.Select(c => c.UserName).ToList() ?? new List<string>()
        };
        throw ResponseFactory.Create<OkResponse<PlaylistResponseDTO>>(responseDto, ["Playlist retrieved successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    // TODO: 😭😭😿😭😭  
    [HttpPost("{playlistId:int}/import-collection/{collection}/{collectionId:int}")]
    public async Task<IActionResult> ImportCollection(int playlistId, string collection, int collectionId)
    {
        Type? T = CollectionStruct.IsValid(collection);
        if (T == null)
            throw ResponseFactory.Create<BadRequestResponse>(["Invalid collection type"]);
        if (T == typeof(Album))
            await playlistService.ImportCollectionToPlaylistAsync<Album>(playlistId, collectionId,
                (await CheckAccessFeatures([])).Id);
        else if (T == typeof(Playlist))
            await playlistService.ImportCollectionToPlaylistAsync<Playlist>(playlistId, collectionId,
                (await CheckAccessFeatures([])).Id);
        else if (T == typeof(Blend))
            await playlistService.ImportCollectionToPlaylistAsync<Blend>(playlistId, collectionId,
                (await CheckAccessFeatures([])).Id);
        throw ResponseFactory.Create<OkResponse>(["Collection was imported to playlist successfully"]);
    }
}