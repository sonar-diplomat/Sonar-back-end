using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Chat;

[Route("api/[controller]")]
[ApiController]
public class ChatController(UserManager<User> userManager) : BaseController(userManager)
{
}
