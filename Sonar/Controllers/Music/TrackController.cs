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
using Sonar.Extensions;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class TrackController(
    UserManager<User> userManager,
    ITrackService trackService,
    IAlbumService albumService,
    ISettingsService settingsService,
    IShareService shareService,
    IUserStateService userStateService) : ShareController<Track>(userManager, shareService)
{

    /// <summary>
    /// Streams a track's audio content with support for TimeSpan-based positioning and optional download.
    /// </summary>
    /// <param name="trackId">The ID of the track to stream.</param>
    /// <param name="startPosition">Optional. The start position as TimeSpan (e.g., "00:01:30" for 1 minute 30 seconds). If not provided, uses queue position if track matches current queue track.</param>
    /// <param name="length">Optional. The length to stream as TimeSpan. If not provided, streams from startPosition to end.</param>
    /// <param name="download">Optional. If true, sets Content-Disposition to attachment for download.</param>
    /// <returns>Audio file stream with appropriate content type.</returns>
    /// <response code="200">Full audio stream returned.</response>
    /// <response code="206">Partial content returned (range request).</response>
    /// <response code="404">Track not found.</response>
    /// <response code="401">User not authenticated or lacks 'ListenContent' access feature.</response>
    /// <remarks>
    /// Supports TimeSpan-based positioning for seeking within the audio file.
    /// If startPosition is not provided and the track matches the user's current queue track, the queue position will be used.
    /// Requires 'ListenContent' access feature.
    /// </remarks>
    [HttpGet("{trackId}/stream")]
    [Authorize]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status206PartialContent)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StreamMusic(
        int trackId,
        [FromQuery] TimeSpan? startPosition = null,
        [FromQuery] TimeSpan? length = null,
        [FromQuery] bool download = false)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ListenContent]);
        int settingsId = user.SettingsId;

        Settings settings = await settingsService.GetByIdValidatedAsync(settingsId);

        if (!startPosition.HasValue)
        {
            UserState userState = await userStateService.GetByUserIdValidatedAsync(user.Id);
            if (userState.Queue?.CurrentTrackId == trackId)
            {
                startPosition = userState.Queue.Position;
            }
        }

        MusicStreamResultDTO? result = await trackService.GetMusicStreamAsync(trackId, startPosition, length, settings.PreferredPlaybackQualityId, user.Id);

        if (result == null) throw ResponseFactory.Create<NotFoundResponse>([$"Track with ID {trackId} not found"]);

        result.GetStreamDetails(out Stream stream, out string contentType, out bool enableRangeProcessing);

        if (download)
            // TODO: Validate User pack
            Response.Headers.Append("Content-Disposition", "attachment; filename=random-track.mp3");

        return File(stream, contentType, enableRangeProcessing);
    }

    /// <summary>
    /// Deletes a track from the platform.
    /// </summary>
    /// <param name="trackId">The ID of the track to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Track deleted successfully.</response>
    /// <response code="401">User not authorized (must be distributor).</response>
    /// <response code="404">Track not found.</response>
    [HttpDelete("{trackId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrack(int trackId)
    {
        await this.CheckDistributorAsync();
        await trackService.DeleteAsync(trackId);
        throw ResponseFactory.Create<OkResponse>([$"Track with ID {trackId} successfully deleted"]);
    }

    /// <summary>
    /// Updates track metadata information.
    /// </summary>
    /// <param name="trackId">The ID of the track to update.</param>
    /// <param name="dto">Updated track information including title, explicit flag, and driving noises flag.</param>
    /// <returns>Updated track entity.</returns>
    /// <response code="200">Track updated successfully.</response>
    /// <response code="401">User not authorized (must be distributor).</response>
    /// <response code="404">Track not found.</response>
    [HttpPut("{trackId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<Track>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<Track> UpdateTrackInfo(int trackId, [FromBody] UpdateTrackDTO dto)
    {
        await this.CheckDistributorAsync();
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        track.Title = dto.Title ?? track.Title;
        track.IsExplicit = dto.IsExplicit ?? track.IsExplicit;
        track.DrivingDisturbingNoises = dto.DrivingDisturbingNoises ?? track.DrivingDisturbingNoises;
        
        // Update Genre if provided
        if (dto.GenreId.HasValue)
        {
            track.GenreId = dto.GenreId.Value;
        }
        
        // Update MoodTags if provided
        if (dto.MoodTagIds != null)
        {
            // Validate mood tag count (0-3)
            if (dto.MoodTagIds.Count() > 3)
            {
                throw ResponseFactory.Create<BadRequestResponse>(["Mood tags cannot exceed 3"]);
            }
            
            await trackService.UpdateMoodTagsAsync(trackId, dto.MoodTagIds);
        }
        
        // TODO: create DTO
        track = await trackService.UpdateAsync(track);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track updated successfully"]);
    }

    /// <summary>
    /// Updates the audio file for a track at a specific playback quality.
    /// </summary>
    /// <param name="trackId">The ID of the track to update.</param>
    /// <param name="dto">Update data including playback quality ID and audio file.</param>
    /// <returns>Success response upon file update.</returns>
    /// <response code="200">Track audio file updated successfully.</response>
    /// <response code="401">User not authorized (must be distributor).</response>
    /// <response code="404">Track not found.</response>
    [HttpPut("{trackId:int}/audio-file")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task UpdateTrackFile(int trackId, [FromForm] UpdateTrackFileDTO dto)
    {
        await this.CheckDistributorAsync();
        await trackService.UpdateTrackFileAsync(trackId, dto.PlaybackQualityId, dto.File);
        throw ResponseFactory.Create<OkResponse>(["Track audio file updated successfully"]);
    }

    /// <summary>
    /// Retrieves detailed information about a specific track.
    /// </summary>
    /// <param name="trackId">The ID of the track to retrieve.</param>
    /// <returns>Track DTO with full details.</returns>
    /// <response code="200">Track retrieved successfully.</response>
    /// <response code="404">Track not found.</response>
    [HttpGet("{trackId:int}")]
    [ProducesResponseType(typeof(OkResponse<TrackDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackById(int trackId)
    {
        // Try to get userId if user is authenticated, but don't require authentication
        int? userId = null;
        try
        {
            User? user = await GetUserByJwt();
            userId = user?.Id;
        }
        catch
        {
            // User is not authenticated, userId remains null
        }

        TrackDTO trackDto = await trackService.GetTrackDtoAsync(trackId, userId);
        throw ResponseFactory.Create<OkResponse<TrackDTO>>(trackDto, ["Track successfully retrieved"]);
    }

    /// <summary>
    /// Updates the visibility status of a track (e.g., public, private, unlisted).
    /// </summary>
    /// <param name="trackId">The ID of the track to update.</param>
    /// <param name="visibilityStatusId">The new visibility status ID.</param>
    /// <returns>Success response upon status update.</returns>
    /// <response code="200">Track visibility updated successfully.</response>
    /// <response code="401">User not authorized (must be distributor).</response>
    /// <response code="404">Track not found.</response>
    [HttpPut("{trackId:int}/visibility")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTrackVisibilityStatus(int trackId, [FromQuery] int visibilityStatusId)
    {
        await this.CheckDistributorAsync();
        await trackService.UpdateVisibilityStatusAsync(trackId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>(["Track visibility status was changed successfully"]);
    }

    /// <summary>
    /// Toggles a track's favorite status for the authenticated user.
    /// </summary>
    /// <param name="trackId">The ID of the track to toggle.</param>
    /// <returns>Success response with message indicating whether track was added or removed from favorites.</returns>
    /// <response code="200">Favorite status toggled successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Track not found.</response>
    [HttpPost("{trackId:int}/toggle-favorite")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFavoriteTrack(int trackId)
    {
        User user = await CheckAccessFeatures([]);
        bool isFavorite = await trackService.ToggleFavoriteAsync(trackId, user.LibraryId);
        string message = isFavorite ? "Track added to favorites" : "Track removed from favorites";
        throw ResponseFactory.Create<OkResponse>([message]);
    }

    /// <summary>
    /// Assigns an artist to a track.
    /// </summary>
    /// <param name="trackId">The ID of the track.</param>
    /// <param name="authorDto">Artist information including pseudonym and optional artist ID.</param>
    /// <returns>Success response upon artist assignment.</returns>
    /// <response code="200">Artist assigned successfully.</response>
    /// <response code="401">User not authorized (must be distributor).</response>
    /// <response code="404">Track not found.</response>
    /// <response code="400">Invalid request (track must belong to an album, artist already assigned).</response>
    [HttpPost("{trackId:int}/artist")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignArtistToTrack(int trackId, [FromBody] AuthorDTO authorDto)
    {
        Distributor distributor = await this.CheckDistributorAsync();

        // Проверяем, что трек принадлежит текущему дистрибьютору
        // Получаем трек с коллекциями для проверки альбома
        Track track = await trackService.GetByIdValidatedAsync(trackId);

        // Получаем трек с коллекциями через репозиторий
        Track? trackWithCollections = await (trackService as ITrackService)!
            .GetByIdAsync(trackId);

        if (trackWithCollections == null)
            throw ResponseFactory.Create<NotFoundResponse>([$"Track with ID {trackId} not found"]);

        // Загружаем коллекции трека
        trackWithCollections = await trackService
            .GetByIdValidatedAsync(trackId);

        // Получаем альбом трека
        Album? album = null;
        if (trackWithCollections.Collections != null)
        {
            album = trackWithCollections.Collections.OfType<Album>().FirstOrDefault();
        }

        if (album == null)
            throw ResponseFactory.Create<BadRequestResponse>(["Track must belong to an album"]);

        // Проверяем права доступа через альбом
        Album albumWithDistributor = await albumService.GetValidatedIncludeDistributorAsync(album.Id);
        if (albumWithDistributor.DistributorId != distributor.Id)
            throw ResponseFactory.Create<UnauthorizedResponse>(["Track does not belong to your distributor"]);

        await trackService.AssignArtistToTrackAsync(trackId, authorDto);
        throw ResponseFactory.Create<OkResponse>(["Artist assigned to track successfully"]);
    }

    /// <summary>
    /// Streams a track's audio content for distributors, ignoring visibility state.
    /// </summary>
    /// <param name="trackId">The ID of the track to stream.</param>
    /// <param name="startPosition">Optional. The start position as TimeSpan (e.g., "00:01:30" for 1 minute 30 seconds).</param>
    /// <param name="length">Optional. The length to stream as TimeSpan. If not provided, streams from startPosition to end.</param>
    /// <param name="download">Optional. If true, sets Content-Disposition to attachment for download.</param>
    /// <returns>Audio file stream with appropriate content type.</returns>
    /// <response code="200">Full audio stream returned.</response>
    /// <response code="206">Partial content returned (range request).</response>
    /// <response code="404">Track not found.</response>
    /// <response code="401">User not authenticated or track does not belong to distributor.</response>
    /// <remarks>
    /// Requires distributor authentication. Only tracks belonging to the authenticated distributor can be streamed.
    /// Visibility state is ignored for distributors.
    /// </remarks>
    [HttpGet("distributor/{trackId}/stream")]
    [Authorize]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status206PartialContent)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StreamMusicForDistributor(
        int trackId,
        [FromQuery] TimeSpan? startPosition = null,
        [FromQuery] TimeSpan? length = null,
        [FromQuery] bool download = false)
    {
        Distributor distributor = await this.CheckDistributorAsync();
        
        // Use default playback quality (medium quality = 2) for distributors
        const int defaultPlaybackQualityId = 2;
        
        MusicStreamResultDTO? result = await trackService.GetMusicStreamForDistributorAsync(
            trackId, 
            startPosition, 
            length, 
            defaultPlaybackQualityId, 
            distributor.Id);

        if (result == null) 
            throw ResponseFactory.Create<NotFoundResponse>([$"Track with ID {trackId} not found"]);

        result.GetStreamDetails(out Stream stream, out string contentType, out bool enableRangeProcessing);

        if (download)
            Response.Headers.Append("Content-Disposition", "attachment; filename=track.mp3");

        return File(stream, contentType, enableRangeProcessing);
    }

    /// <summary>
    /// Retrieves detailed information about a specific track for distributors, ignoring visibility state.
    /// </summary>
    /// <param name="trackId">The ID of the track to retrieve.</param>
    /// <returns>Track DTO with full details.</returns>
    /// <response code="200">Track retrieved successfully.</response>
    /// <response code="404">Track not found.</response>
    /// <response code="401">User not authenticated or track does not belong to distributor.</response>
    /// <remarks>
    /// Requires distributor authentication. Only tracks belonging to the authenticated distributor can be retrieved.
    /// Visibility state is ignored for distributors.
    /// </remarks>
    [HttpGet("distributor/{trackId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<TrackDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTrackByIdForDistributor(int trackId)
    {
        Distributor distributor = await this.CheckDistributorAsync();
        TrackDTO trackDto = await trackService.GetTrackDtoForDistributorAsync(trackId, distributor.Id);
        throw ResponseFactory.Create<OkResponse<TrackDTO>>(trackDto, ["Track successfully retrieved"]);
    }
}