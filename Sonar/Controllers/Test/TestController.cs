using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Test;

[Route("api/[controller]")]
[ApiController]
public class TestController(
    UserManager<User> userManager,
    IQrCodeService qrCodeService,
    IApiKeyGeneratorService apiKeyGeneratorService


) : BaseController(userManager)
{
    [HttpGet("apikey")]
    public async Task<ActionResult> Ping()
    {
        return Ok(new BaseResponse<string>(await apiKeyGeneratorService.GenerateApiKey(), "API Key generated successfully"));
    }
    
    [HttpGet("qr")]
    public async Task<ActionResult> GetDistributors([FromQuery] string link = "https://www.youtube.com/")
    {
        string svg = await qrCodeService.GenerateQrCode(link);
        return Content(svg, "image/svg+xml");
    }
}
