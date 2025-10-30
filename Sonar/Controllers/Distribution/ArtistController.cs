using Application.Abstractions.Interfaces.Services.Utilities;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class ArtistController(UserManager<User> userManager, IShareService shareService) : ShareController<Artist>(userManager, shareService)
{
}