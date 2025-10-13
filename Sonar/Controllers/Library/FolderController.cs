using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Library;

[Route("api/[controller]")]
[ApiController]
public class FolderController(UserManager<User> userManager) : BaseController(userManager)
{
}
