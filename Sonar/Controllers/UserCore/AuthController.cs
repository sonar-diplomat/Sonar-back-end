using Application.DTOs;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly

    public AuthController()
    {

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDTO model)
    {
        User user = new()
        {

        };
        var result = await _userManager.CreateAsync(user, model.);
        if (result.Succeeded)
        {
            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");
            return Ok(new { message = "Registration successful" });
        }



    }



}

