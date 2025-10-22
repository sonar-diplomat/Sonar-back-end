using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Response;
using Entities.Models.Chat;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SysFile = System.IO.File;

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
        throw ResponseFactory.Create<OkResponse<string>>(await apiKeyGeneratorService.GenerateApiKey(),
            ["API Key generated successfully"]);
    }

    [HttpGet("qr")]
    public async Task<ActionResult> GetDistributors([FromQuery] string link = "https://www.youtube.com/")
    {
        string svg = await qrCodeService.GenerateQrCode(link);
        throw ResponseFactory.Create<OkResponse<string>>(svg, ["image/svg+xml"]);
    }

    [HttpGet("appdata")]
    public async Task<ActionResult> CheckAppData()
    {
        string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "app/data");

        // Если нет папки — создаём
        if (!Directory.Exists(dataPath)) return NotFound("(");

        // ✅ Пример записи файла в volume
        string testFilePath = Path.Combine(dataPath, "test.txt");
        await SysFile.WriteAllTextAsync(testFilePath, $"Created at {DateTime.Now}\n");
        throw ResponseFactory.Create<OkResponse>(["File read successfully"]);
    }

    [HttpGet("error")]
    public async Task<ActionResult> GenerateError()
    {
        throw ResponseFactory.Create<BadRequestResponse>(["Test exception"]);
    }

    [HttpGet("ok")]
    public async Task<ActionResult> GenerateOk()
    {
        throw ResponseFactory.Create<OkResponse>(["Test exception"]);
    }

    [HttpGet("okdata")]
    public async Task<ActionResult> GenerateOkWithData()
    {
        IEnumerable<Message> messages =
        [
            new()
            {
                Id = 1,
                ChatId = 1
            },
            new()
            {
                Id = 2,
                ChatId = 2
            }
        ];
        throw ResponseFactory.Create<OkResponse<IEnumerable<Message>>>(messages, ["Test exception"]);
    }
}