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
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        IEnumerable<User> users = (await userService.GetAllAsync()).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<User>>>(users, ["Users retrieved successfully"]);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        User user = await userService.GetByIdAsync(id);
        NonSensetiveUserDTO userDto = new()
        {
            Biography = user.Biography,
            PublicIdentifier = user.PublicIdentifier,
            RegistrationDate = user.RegistrationDate,
            AvatarImage = user.AvatarImage,
            Posts = user.Posts,
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
        throw ResponseFactory.Create<OkResponse<User>>(user, ["User updated successfully"]);
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
        throw ResponseFactory.Create<OkResponse>([$"User visibility status was changed successfully"]);
    }
}