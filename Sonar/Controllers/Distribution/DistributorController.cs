using Application.Abstractions.Interfaces.Services.Utilities;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class DistributorController(
    UserManager<User> userManager,
    IQrCodeService qrCodeService
) : BaseController(userManager)
{
    [HttpGet("qr")]
    public async Task<ActionResult> GetDistributors([FromQuery] string link = "https://www.youtube.com/")
    {
        string svg = await qrCodeService.GenerateQrCode(link);
        return Content(svg, "image/svg+xml");
    }
}
