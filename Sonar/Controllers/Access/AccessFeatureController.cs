using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessFeatureController(UserManager<User> userManager) : BaseController(userManager)
{
    [Authorize]
    private bool AccessFeatureExists(int id)
    {
        throw new NotImplementedException();
    }
}
