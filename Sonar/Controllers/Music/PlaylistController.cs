using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
using Application.DTOs.Music;
using Application.Extensions;
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
    /// <summary>
    /// Creates a new playlist for the authenticated user.
    /// </summary>
    /// <param name="dto">Playlist creation DTO containing name, optional cover image, and other metadata.</param>
    /// <returns>Created playlist response DTO with playlist details.</returns>
    /// <response code="200">Playlist created successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="400">Invalid playlist data.</response>
    [HttpPost("create")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<PlaylistResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlaylist([FromForm] CreatePlaylistDTO dto)
    {
        User user = await CheckAccessFeatures([]);
        Playlist playlist = await playlistService.CreatePlaylistAsync(user.Id, dto);
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        PlaylistResponseDTO responseDto = new()
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CoverId = playlist.CoverId,
            CreatorName = playlist.Creator?.UserName ?? string.Empty,
            TrackCount = playlist.Tracks?.Count ?? 0,
            ContributorNames = playlist.Contributors?.Select(c => c.UserName).ToList() ?? new List<string?>()
        };
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        throw ResponseFactory.Create<OkResponse<PlaylistResponseDTO>>(responseDto, ["Playlist was created successfully"]);
    }

    /// <summary>
    /// Deletes a playlist owned by the authenticated user.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Playlist deleted successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator.</response>
    /// <response code="404">Playlist not found.</response>
    [HttpDelete("{playlistId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlaylist(int playlistId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.DeleteAsync(playlistId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist was deleted successfully"]);
    }

    /// <summary>
    /// Updates the name of a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist to rename.</param>
    /// <param name="newName">The new name for the playlist.</param>
    /// <returns>Updated playlist response DTO.</returns>
    /// <response code="200">Playlist name updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator or contributor.</response>
    /// <response code="404">Playlist not found.</response>
    [HttpPut("{playlistId:int}/update-name")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<PlaylistResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlaylistName(int playlistId, [FromQuery] string newName)
    {
        User user = await CheckAccessFeatures([]);
        Playlist playlist = await playlistService.UpdateNameAsync(playlistId, user.Id, newName);
        PlaylistResponseDTO responseDto = new()
        {
            Id = playlist.Id,
            Name = playlist.Name,
            VisibilityStateId = playlist.VisibilityStateId,
            CoverId = playlist.CoverId,
            CreatorName = playlist.Creator?.UserName ?? string.Empty,
            TrackCount = playlist.Tracks?.Count ?? 0,
            ContributorNames = playlist.Contributors?.Select(c => c.UserName).ToList() ?? new List<string>()
        };
        throw ResponseFactory.Create<OkResponse<PlaylistResponseDTO>>(responseDto, ["Playlist name was updated successfully"]);
    }

    /// <summary>
    /// Updates the cover image of a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="coverFile">The new cover image file.</param>
    /// <returns>Success response upon cover update.</returns>
    /// <response code="200">Playlist cover updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator or contributor.</response>
    /// <response code="404">Playlist not found.</response>
    /// <response code="400">Invalid image file.</response>
    [HttpPut("{playlistId:int}/update-cover")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePlaylistCover(int playlistId, [FromForm] IFormFile coverFile)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.UpdatePlaylistCoverAsync(playlistId, user.Id, coverFile);
        throw ResponseFactory.Create<OkResponse>(["Playlist cover was updated successfully"]);
    }

    /// <summary>
    /// Adds a contributor to a playlist, allowing them to edit the playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="contributorId">The ID of the user to add as a contributor.</param>
    /// <returns>Success response upon adding contributor.</returns>
    /// <response code="200">Contributor added successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator.</response>
    /// <response code="404">Playlist or user not found.</response>
    [HttpPost("{playlistId:int}/add-contributor/{contributorId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddContributor(int playlistId, int contributorId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.AddContributorAsync(playlistId, contributorId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist contributor was added successfully"]);
    }

    /// <summary>
    /// Removes a contributor from a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="contributorId">The ID of the contributor to remove.</param>
    /// <returns>Success response upon removing contributor.</returns>
    /// <response code="200">Contributor removed successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator.</response>
    /// <response code="404">Playlist or contributor not found.</response>
    [HttpDelete("{playlistId:int}/delete-contributor/{contributorId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveContributor(int playlistId, int contributorId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.RemoveContributorAsync(playlistId, contributorId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Playlist contributor was removed successfully"]);
    }

    /// <summary>
    /// Adds a track to a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="trackId">The ID of the track to add.</param>
    /// <returns>Success response upon adding track.</returns>
    /// <response code="200">Track added to playlist successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator or contributor.</response>
    /// <response code="404">Playlist or track not found.</response>
    [HttpPost("{playlistId:int}/add-track/{trackId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTrack(int playlistId, int trackId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.AddTrackToPlaylistAsync(playlistId, trackId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Track was added to playlist successfully"]);
    }

    /// <summary>
    /// Removes a track from a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="trackId">The ID of the track to remove.</param>
    /// <returns>Success response upon removing track.</returns>
    /// <response code="200">Track removed from playlist successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator or contributor.</response>
    /// <response code="404">Playlist or track not found.</response>
    [HttpDelete("{playlistId:int}/remove-track/{trackId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTrack(int playlistId, int trackId)
    {
        User user = await CheckAccessFeatures([]);
        await playlistService.RemoveTrackFromPlaylistAsync(playlistId, trackId, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Track was removed from playlist successfully"]);
    }

    /// <summary>
    /// Retrieves tracks in a playlist using cursor-based pagination.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="after">Optional cursor for pagination (ID of the last track from previous page).</param>
    /// <param name="limit">Number of tracks to retrieve (default: 20).</param>
    /// <returns>Cursor-paginated list of track DTOs.</returns>
    /// <response code="200">Playlist tracks retrieved successfully.</response>
    /// <response code="404">Playlist not found.</response>
    [HttpGet("{playlistId}/tracks")]
    [ProducesResponseType(typeof(OkResponse<CursorPageDTO<TrackDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlaylistWithCursor(int playlistId, [FromQuery] string? after,
        [FromQuery] int limit = 20)
    {
        CursorPageDTO<TrackDTO> result = await playlistService.GetPlaylistTracksAsync(playlistId, after, limit);
        throw ResponseFactory.Create<OkResponse<CursorPageDTO<TrackDTO>>>(result,
            ["Playlist tracks retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves a specific playlist by its ID with basic information.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist to retrieve.</param>
    /// <returns>Playlist response DTO with playlist details.</returns>
    /// <response code="200">Playlist retrieved successfully.</response>
    /// <response code="404">Playlist not found.</response>
    [HttpGet("{playlistId:int}")]
    [ProducesResponseType(typeof(OkResponse<PlaylistResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlaylistById(int playlistId)
    {
        // Get playlist with VisibilityState included for validation
        Playlist playlist = await playlistService.GetByIdWithVisibilityStateAsync(playlistId);

        // Validate visibility before returning playlist data
        playlist.VisibilityState.ValidateVisibility("Playlist", playlistId);

        PlaylistResponseDTO responseDto = new()
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CoverId = playlist.CoverId,
            CreatorName = playlist.Creator?.UserName ?? string.Empty,
            TrackCount = playlist.Tracks?.Count ?? 0,
            ContributorNames = playlist.Contributors?.Select(c => c.UserName).ToList() ?? new List<string>()
        };
        throw ResponseFactory.Create<OkResponse<PlaylistResponseDTO>>(responseDto, ["Playlist retrieved successfully"]);
    }

    /// <summary>
    /// Imports all tracks from another collection (album, playlist, or blend) into this playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist to import into.</param>
    /// <param name="collection">The type of collection to import from ('album', 'playlist', or 'blend').</param>
    /// <param name="collectionId">The ID of the collection to import from.</param>
    /// <returns>Success response upon importing collection.</returns>
    /// <response code="200">Collection imported successfully.</response>
    /// <response code="400">Invalid collection type.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User is not the playlist creator or contributor.</response>
    /// <response code="404">Playlist or source collection not found.</response>
    /// <remarks>
    /// Valid collection types: 'album', 'playlist', 'blend'.
    /// TODO: 😭😭😿😭😭 (requires refactoring/cleanup)
    /// </remarks>
    [HttpPost("{playlistId:int}/import-collection/{collection}/{collectionId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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