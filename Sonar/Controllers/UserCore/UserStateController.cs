using System.Security.Claims;
using Application.Abstractions.Interfaces.Services;
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
    /// Adds tracks or collections to the user's playback queue.
    /// </summary>
    /// <returns>Success response upon adding to queue.</returns>
    /// <response code="501">Not yet implemented.</response>
    /// <remarks>
    /// This endpoint is currently under development.
    /// Will support adding individual tracks or entire collections to the queue.
    /// </remarks>
    [HttpPost("queue")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> AddToQueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes tracks or collections from the user's playback queue.
    /// </summary>
    /// <returns>Success response upon removal from queue.</returns>
    /// <response code="501">Not yet implemented.</response>
    /// <remarks>
    /// This endpoint is currently under development.
    /// </remarks>
    [HttpDelete("queue")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> DeleteFromQueue()
    {
        throw new NotImplementedException();
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