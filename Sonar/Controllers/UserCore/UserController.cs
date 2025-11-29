using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
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
public class UserController(
    IUserService userService,
    UserManager<User> userManager,
    IShareService shareService
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
    [ProducesResponseType(typeof(OkResponse<NonSensetiveUserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUser(int userId)
    {
        User user = await userService.GetByIdValidatedAsync(userId);
        NonSensetiveUserDTO userDto = new()
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
        throw ResponseFactory.Create<OkResponse<NonSensetiveUserDTO>>(userDto, ["User retrieved successfully"]);
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
}