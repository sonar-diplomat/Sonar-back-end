using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Exception;
using Entities.Models.Distribution;
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

    [HttpGet("appdata")]
    public async Task<ActionResult> CheckAppData()
    {
        string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "app/data");

        // Если нет папки — создаём
        if (!Directory.Exists(dataPath)) return NotFound("(");

        // ✅ Пример записи файла в volume
        string testFilePath = Path.Combine(dataPath, "test.txt");
        System.IO.File.WriteAllText(testFilePath, $"Created at {DateTime.Now}\n");
        return Ok();
    }

    [HttpGet("error")]
    public async Task<ActionResult> GenerateError()
    {
        throw AppExceptionFactory.Create<BadRequestException>(["Test exception"]);
    }

    [HttpGet("correct-answer")]
    public async Task<ActionResult> CorrectAnswer()
    {

        return Ok(new BaseResponse<string>("", "This is a correct answer"));
    }
}
