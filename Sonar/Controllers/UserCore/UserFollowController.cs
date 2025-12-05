using Application.Abstractions.Interfaces.Services.UserCore;
using Application.DTOs.User;
using Application.Response;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sonar.Controllers;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class UserFollowController(
    IUserFollowService userFollowService,
    UserManager<User> userManager
) : BaseController(userManager)
{
    private readonly IUserFollowService userFollowService = userFollowService;

    /// <summary>
    /// Follow a user.
    /// </summary>
    /// <param name="userId">The ID of the user to follow.</param>
    /// <returns>Success response.</returns>
    /// <response code="200">Successfully followed the user.</response>
    /// <response code="400">Bad request (e.g., already following, cannot follow yourself).</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("{userId:int}/follow")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FollowUser(int userId)
    {
        User currentUser = await GetUserByJwt();
        await userFollowService.FollowAsync(currentUser.Id, userId);
        throw ResponseFactory.Create<OkResponse>(["Successfully followed the user"]);
    }

    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <param name="userId">The ID of the user to unfollow.</param>
    /// <returns>Success response.</returns>
    /// <response code="200">Successfully unfollowed the user.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">User not found or not following.</response>
    [HttpDelete("{userId:int}/unfollow")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnfollowUser(int userId)
    {
        User currentUser = await GetUserByJwt();
        await userFollowService.UnfollowAsync(currentUser.Id, userId);
        throw ResponseFactory.Create<OkResponse>(["Successfully unfollowed the user"]);
    }

    /// <summary>
    /// Get followers of a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>List of followers.</returns>
    /// <response code="200">Followers retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("{userId:int}/followers")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<UserFollowerDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFollowers(int userId)
    {
        IEnumerable<UserFollowerDTO> followers = await userFollowService.GetFollowersAsync(userId);
        throw ResponseFactory.Create<OkResponse<IEnumerable<UserFollowerDTO>>>(followers, ["Followers retrieved successfully"]);
    }

    /// <summary>
    /// Get users that a user is following.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>List of users being followed.</returns>
    /// <response code="200">Following list retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("{userId:int}/following")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<UserFollowingDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFollowing(int userId)
    {
        IEnumerable<UserFollowingDTO> following = await userFollowService.GetFollowingAsync(userId);
        throw ResponseFactory.Create<OkResponse<IEnumerable<UserFollowingDTO>>>(following, ["Following list retrieved successfully"]);
    }

    /// <summary>
    /// Get mutual follows (friends) of a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>List of mutual follows (friends).</returns>
    /// <response code="200">Mutual follows retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("{userId:int}/mutual")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<UserFriendDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMutualFollows(int userId)
    {
        IEnumerable<User> mutualFollows = await userFollowService.GetMutualFollowsAsync(userId);
        IEnumerable<UserFriendDTO> dtos = mutualFollows.Select(u => new UserFriendDTO
        {
            Id = u.Id,
            UserName = u.UserName ?? string.Empty,
            PublicIdentifier = u.PublicIdentifier,
            AvatarImageId = u.AvatarImageId
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<UserFriendDTO>>>(dtos, ["Mutual follows retrieved successfully"]);
    }
}

