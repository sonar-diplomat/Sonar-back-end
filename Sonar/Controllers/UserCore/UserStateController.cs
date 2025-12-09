using System.Security.Claims;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Music;
using Application.DTOs.User;
using Application.Response;
using Entities.Enums;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class UserStateController(
    UserManager<User> userManager,
    IUserStateService userStateService,
    IUserSessionService userSessionService
) : BaseController(userManager)
{
    /// <summary>
    /// Updates the current playback position for the authenticated user.
    /// </summary>
    /// <param name="position">The new playback position as a TimeSpan.</param>
    /// <returns>Success response upon position update.</returns>
    /// <response code="200">Current position updated successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ListenContent' access feature.</response>
    /// <remarks>
    /// Requires 'ListenContent' access feature.
    /// TODO: Consider replacing TimeSpan with byte offset for better precision.
    /// </remarks>
    [HttpPut("current-position")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCurrentPosition(TimeSpan position)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ListenContent]);
        await userStateService.UpdateCurrentPositionAsync(user.UserStateId, position);
        throw ResponseFactory.Create<OkResponse>(["Current position updated successfully."]);
    }

    /// <summary>
    /// Updates what the user is currently listening to.
    /// </summary>
    /// <param name="trackId">The ID of the track being played.</param>
    /// <param name="collectionId">Optional. The ID of the collection (album, playlist, blend) the track is from.</param>
    /// <returns>Success response upon update.</returns>
    /// <response code="200">Listening target updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Track or collection not found.</response>
    [HttpPut("listening/{trackId:int}/{collectionId:int?}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult>
        UpdateListeningTarget(int trackId, int? collectionId)
    {
        UserState userState = await userStateService.GetByUserIdValidatedAsync((await CheckAccessFeatures([])).Id);
        await userStateService.UpdateListeningTargetAsync(userState.Id, trackId, collectionId);
        throw ResponseFactory.Create<OkResponse>(["Listening target updated successfully."]);
    }

    /// <summary>
    /// Adds tracks to the user's playback queue.
    /// </summary>
    /// <param name="trackIds">List of track IDs to add to the queue.</param>
    /// <returns>Success response upon adding to queue.</returns>
    /// <response code="200">Tracks added to queue successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">One or more tracks not found.</response>
    /// <remarks>
    /// Adds the specified tracks to the end of the user's playback queue.
    /// Duplicate tracks will be ignored.
    /// </remarks>
    [HttpPost("queue")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToQueue([FromBody] IEnumerable<int> trackIds)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ListenContent]);
        await userStateService.AddTracksToUserQueueAsync(user.Id, trackIds);
        throw ResponseFactory.Create<OkResponse>(["Tracks added to queue successfully."]);
    }

    /// <summary>
    /// Removes tracks from the user's playback queue.
    /// </summary>
    /// <param name="trackIds">List of track IDs to remove from the queue.</param>
    /// <returns>Success response upon removal from queue.</returns>
    /// <response code="200">Tracks removed from queue successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Queue not found.</response>
    /// <remarks>
    /// Removes the specified tracks from the user's playback queue.
    /// Non-existent tracks in the queue are silently ignored.
    /// </remarks>
    [HttpDelete("queue")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFromQueue([FromBody] IEnumerable<int> trackIds)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ListenContent]);
        await userStateService.RemoveTracksFromUserQueueAsync(user.Id, trackIds);
        throw ResponseFactory.Create<OkResponse>(["Tracks removed from queue successfully."]);
    }

    /// <summary>
    /// Retrieves the current user's playback queue including all tracks.
    /// </summary>
    /// <returns>Queue DTO with position, current track, collection, and all queued tracks.</returns>
    /// <response code="200">Queue retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Queue not found.</response>
    [HttpGet("queue")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<QueueDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserQueue()
    {
        User user = await CheckAccessFeatures([]);
        Queue queue = await userStateService.GetUserQueueAsync(user.Id);
        QueueDTO dto = new()
        {
            Id = queue.Id,
            Position = queue.Position,
            CollectionId = queue.CollectionId,
            CurrentTrackId = queue.CurrentTrackId,
            Tracks = queue.Tracks.Select(t => new TrackDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationInSeconds = (int)(t.Duration?.TotalSeconds ?? 0),
                IsExplicit = t.IsExplicit,
                DrivingDisturbingNoises = t.DrivingDisturbingNoises,
                CoverId = t.CoverId,
                AudioFileId = t.LowQualityAudioFileId,
                Artists = t.TrackArtists.Select(ta => new AuthorDTO
                {
                    Pseudonym = ta.Pseudonym,
                    ArtistId = ta.ArtistId
                }).ToList()
            }).ToList()
        };
        throw ResponseFactory.Create<OkResponse<QueueDTO>>(dto, ["Queue retrieved successfully."]);
    }

    /// <summary>
    /// Completely replaces the user's playback queue with a new list of tracks.
    /// </summary>
    /// <param name="trackIds">List of track IDs to set as the new queue.</param>
    /// <returns>Success response upon queue replacement.</returns>
    /// <response code="200">Queue saved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">One or more tracks not found.</response>
    /// <remarks>
    /// This endpoint overwrites the entire queue. Use POST and DELETE for smaller adjustments.
    /// The order of tracks in the input list will be preserved in the queue.
    /// </remarks>
    [HttpPut("queue")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveQueue([FromBody] IEnumerable<int> trackIds)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ListenContent]);
        await userStateService.SaveUserQueueAsync(user.Id, trackIds);
        throw ResponseFactory.Create<OkResponse>(["Queue saved successfully."]);
    }

    /// <summary>
    /// Updates the user's status (e.g., online, away, do not disturb).
    /// </summary>
    /// <param name="statusId">The ID of the new user status.</param>
    /// <returns>Success response upon status update.</returns>
    /// <response code="200">User status updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Status not found.</response>
    [HttpPut("status/{statusId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserStatus(int statusId)
    {
        int userStateId = (await CheckAccessFeatures([])).UserStateId;
        await userStateService.UpdateUserStatusAsync(userStateId, statusId);
        throw ResponseFactory.Create<OkResponse>(["User status updated successfully."]);
    }

    /// <summary>
    /// Updates the primary session for the authenticated user based on the current device.
    /// </summary>
    /// <returns>Success response upon primary session update.</returns>
    /// <response code="200">Primary session updated successfully.</response>
    /// <response code="400">Device ID claim is missing from JWT.</response>
    /// <response code="401">User not authenticated.</response>
    /// <remarks>
    /// Uses the device ID from the JWT session ID claim (ClaimTypes.Sid).
    /// </remarks>
    [HttpPatch("session")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserPrimarySession()
    {
        User user = await CheckAccessFeatures([]);
        string deviceId = User.FindFirst(ClaimTypes.Sid)?.Value ?? string.Empty;
        if (deviceId == string.Empty)
            throw ResponseFactory.Create<BadRequestResponse>(["Device ID claim is missing."]);
        await userStateService.UpdatePrimarySessionAsync(user.Id, deviceId);
        throw ResponseFactory.Create<OkResponse>(["Primary session updated successfully."]);
    }
}