using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class PlaylistController(UserManager<User> userManager) : BaseController(userManager)
{
}
