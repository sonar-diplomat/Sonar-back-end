using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IUserService userService
)
    : ControllerBase
{
    private readonly UserManager<User> userManager;

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
        User? user = await userManager.GetUserAsync(User);
        if (user == null)
            throw AppExceptionFactory.Create<UnauthorizedException>();

        user = await userService.UpdateUserAsync(user.Id, request);

        return Ok(user);
    }
}
