using System.Drawing.Drawing2D;
using Application.Abstractions.Interfaces.Services.Utilities;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class BlendController(UserManager<User> userManager, IShareService shareService) : ShareController<Blend>(userManager, shareService)
{
}
