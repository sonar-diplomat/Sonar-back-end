using Application.Exception;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

public abstract class BaseController(UserManager<User> userManager) : ControllerBase
{
    [Authorize]
    protected async Task<User> GetUserByJwt()
    {
        User? user = await userManager.GetUserAsync(User);
        return user ?? throw AppExceptionFactory.Create<UnauthorizedException>();
    }
}
