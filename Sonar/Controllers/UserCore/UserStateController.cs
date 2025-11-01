using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Enums;
using Entities.Models.UserCore;
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
    [HttpPut("current-position")]
    public async Task<IActionResult>
        UpdateCurrentPosition(TimeSpan position) // TODO: Consider replacing TimeSpan with bytes
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ListenContent]);
        await userStateService.UpdateCurrentPositionAsync(user.UserStateId, position);
        throw ResponseFactory.Create<OkResponse>(["Current position updated successfully."]);
    }

    [HttpPut("current-track")]
    public async Task<IActionResult> UpdateListeningTarget() // Current collectionId and/or trackId
    {
        throw new NotImplementedException();
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

    [HttpPatch("change-session")]
    public async Task<IActionResult> ChangeUserPrimarySession()
    {
        throw new NotImplementedException();
    }
}