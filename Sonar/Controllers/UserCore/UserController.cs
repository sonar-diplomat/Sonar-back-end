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
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserAdminDTO>>> GetUsers()
    {
        await CheckAccessFeatures([AccessFeatureStruct.IamAGod]);
        IEnumerable<User> users = (await userService.GetAllAsync()).ToList();
        IEnumerable<UserAdminDTO> dtos = users.Select(u => new UserAdminDTO
        {
            Id = u.Id,
            UserName = u.UserName,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Login = u.Login,
            Email = u.Email,
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

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<User>> GetUser(int userId)
    {
        User user = await userService.GetByIdValidatedAsync(userId);
        NonSensetiveUserDTO userDto = new()
        {
            Biography = user.Biography,
            PublicIdentifier = user.PublicIdentifier,
            RegistrationDate = user.RegistrationDate,
            AvatarImage = user.AvatarImage,
            AccessFeatures = user.AccessFeatures
        };
        throw ResponseFactory.Create<OkResponse<NonSensetiveUserDTO>>(userDto, ["User retrieved successfully"]);
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(UserUpdateDTO request)
    {
        User user = await CheckAccessFeatures([]);
        user = await userService.UpdateUserAsync(user.Id, request);
        UserResponseDTO dto = new UserResponseDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            PublicIdentifier = user.PublicIdentifier,
            Biography = user.Biography,
            RegistrationDate = user.RegistrationDate,
            AvatarUrl = user.AvatarImage?.Url ?? string.Empty
        };
        throw ResponseFactory.Create<OkResponse<UserResponseDTO>>(dto, ["User updated successfully"]);
    }

    [HttpPost("update-avatar")]
    [Consumes("multipart/form-data")]
    [Authorize]
    public async Task<IActionResult> Index([FromForm] IFormFile file)
    {
        User user = await CheckAccessFeatures([]);
        await userService.UpdateAvatar(user.Id, file);
        throw ResponseFactory.Create<OkResponse>(["File uploaded successfully"]);
    }

    [HttpPut("{collectionId:int}/visibility")]
    [Authorize]
    public async Task<IActionResult> UpdateVisibilityStatus(int collectionId, int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
        await userService.UpdateVisibilityStatusAsync(collectionId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>(["User visibility status was changed successfully"]);
    }
}