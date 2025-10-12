using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FileModel = Entities.Models.File.File;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IUserService userService,
    UserManager<User> userManager,
    IFileService fileService,
    IFileTypeService fileTypeService
)
    : BaseController(userManager)
{
    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        IEnumerable<User> users = await userService.GetAllAsync();
        return Ok(users);
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        User user = await userService.GetByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("update")]
    public async Task<IActionResult> PatchUser(UserUpdateDTO request)
    {
        User user = await GetUserByJwt();
        user = await userService.UpdateUserAsync(user.Id, request);
        return Ok(user);
    }

    [HttpPost("update-avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Index([FromForm] IFormFile file)
    {
        FileModel fileModel = await fileService.UploadFileAsync(
            await fileTypeService.GetByNameAsync("image"),
            file
        );

        return Ok(new BaseResponse<FileModel>(fileModel, "File uploaded successfully"));
    }
}
