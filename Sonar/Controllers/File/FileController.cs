using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.File;

[Route("api/[controller]")]
[ApiController]
public class FileController(UserManager<User> userManager) : BaseController(userManager)
{
}
