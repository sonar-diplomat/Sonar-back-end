using Application;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Abstractions.Interfaces.Repository.Music;
using Application.DTOs.User;
using Application.Response;
using Application.Extensions;
using Entities.Enums;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IUserService userService,
    UserManager<User> userManager,
    IShareService shareService,
    IUserFollowService userFollowService,
    IPlaylistRepository playlistRepository
)
    : ShareController<User>(userManager, shareService)
{
    /// <summary>
    /// Retrieves all users in the system with full administrative details.
    /// </summary>
    /// <returns>List of user admin DTOs with access features and account information.</returns>
    /// <response code="200">Users retrieved successfully.</response>
    /// <response code="401">User not authorized (requires 'IamAGod' access feature).</response>
    /// <remarks>
    /// This endpoint requires the 'IamAGod' access feature for administrative access.
    /// </remarks>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<UserAdminDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<UserAdminDTO>>> GetUsers()
    {
        await CheckAccessFeatures([AccessFeatureStruct.IamAGod]);
        IEnumerable<User> users = (await userService.GetAllAsync()).ToList();
        IEnumerable<UserAdminDTO> dtos = users.Select(u => new UserAdminDTO
        {
            Id = u.Id,
            UserName = u.UserName!,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Login = u.Login,
            Email = u.Email!,
            PublicIdentifier = u.PublicIdentifier,
            RegistrationDate = u.RegistrationDate,
            EmailConfirmed = u.EmailConfirmed,
            Enabled2FA = u.TwoFactorEnabled,
            AvailableCurrency = u.AvailableCurrency,
            AvatarImageId = u.AvatarImageId,
            VisibilityStateId = u.VisibilityStateId,
            SubscriptionPackId = u.SubscriptionPackId,
            UserStateId = u.UserStateId,
            SettingsId = u.SettingsId,
            InventoryId = u.InventoryId,
            LibraryId = u.LibraryId,
            AccessFeatures = u.AccessFeatures.Select(af => new Application.DTOs.Access.AccessFeatureDTO
            {
                Id = af.Id,
                Name = af.Name
            }).ToList()
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<UserAdminDTO>>>(dtos, ["Users retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves public profile information for a specific user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>Non-sensitive user DTO with public profile information.</returns>
    /// <response code="200">User profile retrieved successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{userId:int}")]
    [ProducesResponseType(typeof(OkResponse<NonSensitiveUserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUser(int userId)
    {
        User user = await userService.GetByIdValidatedAsync(userId);
        NonSensitiveUserDTO userDto = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Login = user.Login,
            Biography = user.Biography,
            PublicIdentifier = user.PublicIdentifier,
            RegistrationDate = user.RegistrationDate,
            AvailableCurrency = user.AvailableCurrency,
            AvatarImageId = user.AvatarImageId,
            AccessFeatures = user.AccessFeatures.Select(af => new Application.DTOs.Access.AccessFeatureDTO
            {
                Id = af.Id,
                Name = af.Name
            }).ToList()
        };
        throw ResponseFactory.Create<OkResponse<NonSensitiveUserDTO>>(userDto, ["User retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves profile information for a specific user by their ID.
    /// Returns only the data needed for displaying the user profile.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>User profile DTO with profile information and statistics.</returns>
    /// <response code="200">User profile retrieved successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{userId:int}/profile")]
    [ProducesResponseType(typeof(OkResponse<UserProfileDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDTO>> GetUserProfile(int userId)
    {
        User user = await userService.GetByIdValidatedAsync(userId);
        
        // Get followers and following counts
        var followers = await userFollowService.GetFollowersAsync(userId);
        var following = await userFollowService.GetFollowingAsync(userId);
        
        // Get public playlists
        var allPlaylists = await playlistRepository.GetAllAsync();
        var userPlaylists = allPlaylists
            .Where(p => p.CreatorId == userId)
            .SnInclude(p => p.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .SnInclude(p => p.Tracks);
        
        var publicPlaylists = VisibilityStateValidator.FilterPlaylistsByVisibility(userPlaylists, null)
            .Select(p => new UserPlaylistDTO
            {
                Id = p.Id,
                Name = p.Name,
                CoverId = p.CoverId,
                TrackCount = p.Tracks != null ? p.Tracks.Count : 0
            })
            .ToList();
        
        UserProfileDTO profileDto = new()
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            PublicIdentifier = user.PublicIdentifier,
            Biography = user.Biography,
            AvatarImageId = user.AvatarImageId,
            ImageUrl = null, // Will be generated on frontend using AvatarImageId
            RegistrationDate = user.RegistrationDate,
            FollowersCount = followers.Count(),
            FollowingCount = following.Count(),
            AccessFeatures = user.AccessFeatures.Select(af => new Application.DTOs.Access.AccessFeatureDTO
            {
                Id = af.Id,
                Name = af.Name
            }).ToList(),
            PublicPlaylists = publicPlaylists
        };
        
        throw ResponseFactory.Create<OkResponse<UserProfileDTO>>(profileDto, ["User profile retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves profile information for a specific user by their public identifier.
    /// Returns only the data needed for displaying the user profile.
    /// </summary>
    /// <param name="publicIdentifier">The public identifier of the user to retrieve.</param>
    /// <returns>User profile DTO with profile information and statistics.</returns>
    /// <response code="200">User profile retrieved successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("profile/{publicIdentifier}")]
    [ProducesResponseType(typeof(OkResponse<UserProfileDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDTO>> GetUserProfileByIdentifier(string publicIdentifier)
    {
        User user = await userService.GetByPublicIdentifierValidatedAsync(publicIdentifier);
        
        // Get followers and following counts
        var followers = await userFollowService.GetFollowersAsync(user.Id);
        var following = await userFollowService.GetFollowingAsync(user.Id);
        
        // Get public playlists
        var allPlaylists = await playlistRepository.GetAllAsync();
        var userPlaylists = allPlaylists
            .Where(p => p.CreatorId == user.Id)
            .SnInclude(p => p.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .SnInclude(p => p.Tracks);
        
        var publicPlaylists = VisibilityStateValidator.FilterPlaylistsByVisibility(userPlaylists, null)
            .Select(p => new UserPlaylistDTO
            {
                Id = p.Id,
                Name = p.Name,
                CoverId = p.CoverId,
                TrackCount = p.Tracks != null ? p.Tracks.Count : 0
            })
            .ToList();
        
        UserProfileDTO profileDto = new()
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            PublicIdentifier = user.PublicIdentifier,
            Biography = user.Biography,
            AvatarImageId = user.AvatarImageId,
            ImageUrl = null, // Will be generated on frontend using AvatarImageId
            RegistrationDate = user.RegistrationDate,
            FollowersCount = followers.Count(),
            FollowingCount = following.Count(),
            AccessFeatures = user.AccessFeatures.Select(af => new Application.DTOs.Access.AccessFeatureDTO
            {
                Id = af.Id,
                Name = af.Name
            }).ToList(),
            PublicPlaylists = publicPlaylists
        };
        
        throw ResponseFactory.Create<OkResponse<UserProfileDTO>>(profileDto, ["User profile retrieved successfully"]);
    }

    /// <summary>
    /// Updates the authenticated user's profile information.
    /// </summary>
    /// <param name="request">User update DTO containing the new profile information.</param>
    /// <returns>Updated user response DTO with current profile data.</returns>
    /// <response code="200">User profile updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPut("update")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<UserResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUser(UserUpdateDTO request)
    {
        User user = await CheckAccessFeatures([]);
        user = await userService.UpdateUserAsync(user.Id, request);
        UserResponseDTO dto = new()
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Login = user.Login,
            PublicIdentifier = user.PublicIdentifier,
            Biography = user.Biography,
            RegistrationDate = user.RegistrationDate,
            AvailableCurrency = user.AvailableCurrency,
            AvatarImageId = user.AvatarImageId
        };
        throw ResponseFactory.Create<OkResponse<UserResponseDTO>>(dto, ["User updated successfully"]);
    }

    /// <summary>
    /// Updates the authenticated user's avatar image.
    /// </summary>
    /// <param name="file">The image file to upload as the new avatar.</param>
    /// <returns>Success response upon avatar upload.</returns>
    /// <response code="200">Avatar uploaded successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="400">Invalid file format or file too large.</response>
    [HttpPost("update-avatar")]
    [Consumes("multipart/form-data")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index([FromForm] IFormFile file)
    {
        User user = await CheckAccessFeatures([]);
        await userService.UpdateAvatar(user.Id, file);
        throw ResponseFactory.Create<OkResponse>(["File uploaded successfully"]);
    }

    /// <summary>
    /// Updates the visibility status of a user's collection.
    /// </summary>
    /// <param name="collectionId">The ID of the collection to update visibility for.</param>
    /// <param name="visibilityStatusId">The ID of the new visibility status.</param>
    /// <returns>Success response upon visibility status update.</returns>
    /// <response code="200">Visibility status updated successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ManageContent' access feature.</response>
    /// <response code="404">Collection or visibility status not found.</response>
    /// <remarks>
    /// Requires 'ManageContent' access feature.
    /// </remarks>
    [HttpPut("{collectionId:int}/visibility")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVisibilityStatus(int collectionId, int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
        await userService.UpdateVisibilityStatusAsync(collectionId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>(["User visibility status was changed successfully"]);
    }


    /// <summary>
    /// Removes a friend from the authenticated user's friends list.
    /// </summary>
    /// <param name="friendId">The ID of the friend to remove.</param>
    /// <returns>Success response upon removal.</returns>
    /// <response code="200">Friend removed successfully.</response>
    /// <response code="400">User is not in friends list.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Friend user not found.</response>
    [HttpDelete("friends/{friendId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFriend(int friendId)
    {
        User user = await CheckAccessFeatures([]);
        await userService.RemoveFriendAsync(user.Id, friendId);
        throw ResponseFactory.Create<OkResponse>(["Friend removed successfully"]);
    }

    /// <summary>
    /// Retrieves all friends of the authenticated user.
    /// </summary>
    /// <returns>List of friend DTOs.</returns>
    /// <response code="200">Friends retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("friends")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<UserFriendDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFriends()
    {
        User user = await CheckAccessFeatures([]);
        IEnumerable<User> friends = await userService.GetFriendsAsync(user.Id);
        IEnumerable<UserFriendDTO> dtos = friends.Select(f => new UserFriendDTO
        {
            Id = f.Id,
            UserName = f.UserName ?? string.Empty,
            PublicIdentifier = f.PublicIdentifier,
            AvatarImageId = f.AvatarImageId
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<UserFriendDTO>>>(dtos, ["Friends retrieved successfully"]);
    }
}