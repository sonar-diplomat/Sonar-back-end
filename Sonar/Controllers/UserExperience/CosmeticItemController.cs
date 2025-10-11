using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class CosmeticItemController(UserManager<User> userManager) : BaseController(userManager)
{
}
