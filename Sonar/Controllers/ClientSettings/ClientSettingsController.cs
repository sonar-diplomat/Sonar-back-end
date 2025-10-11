using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;

namespace Sonar.Controllers.ClientSettings;

public class ClientSettingsController(UserManager<User> userManager) : BaseController(userManager)
{
}
