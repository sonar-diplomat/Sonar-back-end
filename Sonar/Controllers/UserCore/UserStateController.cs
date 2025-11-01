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
    // TODO: Consider replacing TimeSpan with bytes
    [HttpPut("current-position")]
    public async Task<IActionResult> UpdateCurrentPosition(TimeSpan position)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ListenContent]);
        await userStateService.UpdateCurrentPositionAsync(user.UserStateId, position);
        throw ResponseFactory.Create<OkResponse>(["Current position updated successfully."]);
    }

    [HttpPut("listening/{trackId:int}/{collectionId:int?}")]
    public async Task<IActionResult>
        UpdateListeningTarget(int trackId, int? collectionId) // Current collectionId and/or trackId
    {
        UserState userState = await userStateService.GetByUserIdValidatedAsync((await CheckAccessFeatures([])).Id);
        await userStateService.UpdateListeningTargetAsync(userState.Id, trackId, collectionId);
        throw ResponseFactory.Create<OkResponse>(["Listening target updated successfully."]);
    }

    [HttpPost("queue")]
    public async Task<IActionResult> AddToQueue() // Collection or track ids
    {
        throw new NotImplementedException();
    }

    [HttpDelete("queue")]
    public async Task<IActionResult> DeleteFromQueue() // Collection or track ids
    {
        throw new NotImplementedException();
    }

    [HttpPut("status/{statusId:int}")]
    public async Task<IActionResult> UpdateUserStatus(int statusId)
    {
        int userStateId = (await CheckAccessFeatures([])).UserStateId;
        await userStateService.UpdateUserStatusAsync(userStateId, statusId);
        throw ResponseFactory.Create<OkResponse>(["User status updated successfully."]);
    }

    [HttpPatch("session")]
    [Authorize]
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