using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
using Application.DTOs.Music;
using Application.Response;
using Application.Services.Utilities;
using Entities.Enums;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class PlaylistController(UserManager<User> userManager, IPlaylistService playlistService, ICollectionService<Playlist> collectionService, IShareService shareService) 
    : CollectionController<Playlist>(userManager, collectionService, shareService)
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistDTO dto)
    {
        User user = await CheckAccessFeatures([]);
        Playlist playlist = await playlistService.CreatePlaylistAsync(user.Id, dto);
        throw ResponseFactory.Create<OkResponse<Playlist>>(playlist, ["Playlist was created successfully"]);
    }

    [HttpDelete("{playlistId:int}")]
    [Authorize]
    public async Task<IActionResult> DeletePlaylist(int playlistId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.DeleteAsync(playlistId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist was deleted successfully"]);
    }

    [HttpPut("{playlistId:int}/update-name")]
    public async Task<IActionResult> UpdatePlaylistName(int playlistId, [FromBody] string newName)
    {
        User user = await CheckAccessFeatures([]);
        Playlist playlist = await playlistService.UpdateNameAsync(playlistId, user.Id, newName);
        throw ResponseFactory.Create<OkResponse<Playlist>>(playlist, ["Playlist name was updated successfully"]);
    }

    [HttpPut("{playlistId:int}/update-cover")]
    public async Task<IActionResult> UpdatePlaylistCover(int playlistId, IFormFile coverFile)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.UpdatePlaylistCoverAsync(playlistId, user.Id, coverFile);
        throw ResponseFactory.Create<OkResponse>(["Playlist cover was updated successfully"]);
    }

    [HttpPost("{playlistId:int}/add-contributor/{contributorId:int}")]
    [Authorize]
    public async Task<IActionResult> AddContributor(int playlistId, int contributorId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.AddContributorAsync(playlistId, contributorId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist contributor was added successfully"]);
    }

    [HttpDelete("{playlistId:int}/delete-contributor/{contributorId:int}")]
    [Authorize]
    public async Task<IActionResult> RemoveContributor(int playlistId, int contributorId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.RemoveContributorAsync(playlistId, contributorId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist contributor was removed successfully"]);
    }

    [HttpPost("{playlistId:int}/add-track/{trackId:int}")]
    [Authorize]
    public async Task<IActionResult> AddTrack(int playlistId, int trackId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.AddTrackToPlaylistAsync(playlistId, trackId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Track was added to playlist successfully"]);
    }

    [HttpDelete("{playlistId:int}/remove-track/{trackId:int}")]
    public async Task<IActionResult> RemoveTrack(int playlistId, int trackId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.RemoveTrackFromPlaylistAsync(playlistId, trackId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Track was removed from playlist successfully"]);
    }

    [HttpGet("{playlistId}/tracks")]
    public async Task<IActionResult> GetPlaylistWithCursor(int playlistId, [FromQuery] string? after,
        [FromQuery] int limit = 20)
    {
        CursorPageDTO<TrackDTO> result = await playlistService.GetPlaylistTracksAsync(playlistId, after, limit);
        throw ResponseFactory.Create<OkResponse<CursorPageDTO<TrackDTO>>>(data: result, ["Playlist tracks retrieved successfully"] );
    }

    [HttpGet("{playlistId:int}")]
    public async Task<IActionResult> GetPlaylistById(int playlistId)
    {
        Playlist playlist = await playlistService.GetByIdValidatedAsync(playlistId);
        throw ResponseFactory.Create<OkResponse<Playlist>>(playlist, ["Playlist retrieved successfully"]);
    }
    
    [HttpPost("{playlistId:int}import-collection/{collection}/{collectionId:int}")]
    public async Task<IActionResult> ImportCollection(int playlistId, string collection, int collectionId)
    {
        Type? T = CollectionStruct.IsValid(collection);
        if (T == null)
            throw ResponseFactory.Create<BadRequestResponse>(["Invalid collection type"]);
        if (T == typeof(Album))
            await playlistService.ImportCollectionToPlaylistAsync<Album>(playlistId, collectionId, userId: (await CheckAccessFeatures([])).Id);
        else if (T == typeof(Playlist))
            await playlistService.ImportCollectionToPlaylistAsync<Playlist>(playlistId, collectionId, userId: (await CheckAccessFeatures([])).Id);
        else if (T == typeof(Blend))
            await playlistService.ImportCollectionToPlaylistAsync<Blend>(playlistId, collectionId, userId: (await CheckAccessFeatures([])).Id);
        throw ResponseFactory.Create<OkResponse>(["Collection was imported to playlist successfully"]);
    }
}