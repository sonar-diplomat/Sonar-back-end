using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.User;
using Application.Response;
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
    IImageFileService imageFileService
)
    : BaseController(userManager)
{
    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        IEnumerable<User> users = (await userService.GetAllAsync()).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<User>>>(users, ["Users retrieved successfully"]);
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        //TODO set to UserPublicDTO
        User user = await userService.GetByIdAsync(id);
        throw ResponseFactory.Create<OkResponse<User>>(user, ["User retrieved successfully"]);
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> PatchUser(UserUpdateDTO request)
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
        //FileModel fileModel = await imageFileService.UploadFileAsync(file);
        await userService.UpdateAvatar(user.Id, file);
        throw ResponseFactory.Create<OkResponse>(["File uploaded successfully"]);
    }
}