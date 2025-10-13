using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class ArtistController(UserManager<User> userManager) : BaseController(userManager)
{
}
