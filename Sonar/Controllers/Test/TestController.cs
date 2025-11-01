using System.Text.Json;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Extensions;
using Application.Response;
using Entities.Models.Chat;
using Entities.Models.Distribution;
using Entities.Models.Library;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Settings = Entities.Models.ClientSettings.Settings;
using SysFile = System.IO.File;

namespace Sonar.Controllers.Test;

[Route("api/[controller]")]
[ApiController]
public class TestController(
    UserManager<User> userManager,
    IShareService shareService,
    IApiKeyGeneratorService apiKeyGeneratorService,
    IImageFileService fileService,
    IDistributorRepository distributorRepository,
    ISettingsService settingsService,
    IFolderService folderService,
    ILibraryService libraryService
) : BaseController(userManager)
{
    # region dist

    [HttpGet("apikey")]
    public async Task<ActionResult> Ping()
    {
        throw ResponseFactory.Create<OkResponse<string>>(await apiKeyGeneratorService.GenerateApiKey(),
            ["API Key generated successfully"]);
    }

    [HttpGet("qr")]
    public async Task<ActionResult> GetDistributors([FromQuery] string link = "https://www.youtube.com/")
    {
        string svg = await shareService.GenerateQrCode(link);
        throw ResponseFactory.Create<OkResponse<string>>(svg, ["image/svg+xml"]);
    }

    [HttpGet("test_include")]
    public async Task<ActionResult> TestInclude()
    {
        Distributor? d = await distributorRepository.Include(d => d.Cover).Include(d => d.License)
            .ThenInclude(l => l.Issuer).GetByIdAsync(4);
        throw ResponseFactory.Create<OkResponse<Distributor>>(d);
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

    [HttpPost("fileuploadtest")]
    public async Task<ActionResult> FileUploadTest(IFormFile file)
    {
        await fileService.UploadFileAsync(file);
        return Ok();
    }

    # endregion

    # region response tests

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

    # endregion

    # region settings and folder tests

    [HttpGet("settings/{settingsId?}")]
    public async Task<ActionResult> GetSettings(int settingsId = 1)
    {
        throw ResponseFactory.Create<OkResponse<Settings>>(await settingsService.GetByIdValidatedFullAsync(settingsId),
            ["Settings retrieved successfully"]);
    }


    [HttpPatch("settings/{settingsId:int}")]
    public async Task<IActionResult> PatchSettings(int settingsId, [FromBody] JsonElement updates)
    {
        await settingsService.PatchAsync(settingsId, updates);
        throw ResponseFactory.Create<OkResponse>(["Settings patched successfully"]);
    }

    # endregion

    # region folder tests

    [HttpGet("folder/{folderId:int}")]
    public async Task<IActionResult> GetFolder(int folderId)
    {
        Folder folder = await folderService.GetByIdValidatedAsync(folderId);
        throw ResponseFactory.Create<OkResponse<Folder>>(folder, ["Successfully retrieved folder"]);
    }

    [HttpGet("folder")]
    public async Task<IActionResult> GetAllFolders()
    {
        IEnumerable<Folder> folders = await folderService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<Folder>>>(folders, ["Successfully retrieved all folders"]);
    }

    [HttpPost("folder")]
    public async Task<IActionResult> CreateFolder()
    {
        int libraryId = 1;
        Folder folder = await folderService.CreateAsync(new Folder
        {
            Name = "Test Folder" + DateTime.Now.Ticks,
            IsProtected = false,
            ParentFolderId = null
        });
        throw ResponseFactory.Create<OkResponse<Folder>>(folder, ["Successfully created folder"]);
    }

    [HttpPost("library/default")]
    public async Task<IActionResult> CreateDefaultFolder()
    {
        throw ResponseFactory.Create<OkResponse<Entities.Models.Library.Library>>(
            await libraryService.CreateDefaultAsync(), ["Successfully created default folder"]);
    }

    # endregion
}