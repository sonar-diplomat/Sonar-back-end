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
using System.Text.Json;
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
    ILibraryService libraryService,
    IVisibilityStateService visibilityStateService,
    IImageFileService imageFileService
) : BaseController(userManager)
{
    #region dist

    [HttpPost("IspottedABigFuckingTankinYoureArea")]
    public async Task<IActionResult> IspottedABigFuckingTankinYoureArea([FromForm] IFormFile tank)
    {
        throw ResponseFactory.Create<OkResponse>(["UUUUUUUUUUUUUUUUUOOOOOOOOOOOOOOOOOOAAAAAAAAAAAAAAAAAA (►__◄)"]);
    }

    /// <summary>
    /// [TEST] Generates a test API key.
    /// </summary>
    /// <returns>Generated API key string.</returns>
    /// <response code="200">API key generated successfully.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// </remarks>
    [HttpGet("apikey")]
    [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Ping()
    {
        throw ResponseFactory.Create<OkResponse<string>>(await apiKeyGeneratorService.GenerateApiKey(),
            ["API Key generated successfully"]);
    }

    /// <summary>
    /// [TEST] Generates a QR code for a given link.
    /// </summary>
    /// <param name="link">The URL to encode in the QR code (default: YouTube homepage).</param>
    /// <returns>QR code as SVG string.</returns>
    /// <response code="200">QR code generated successfully.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// </remarks>
    [HttpGet("qr")]
    [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetDistributors([FromQuery] string link = "https://www.youtube.com/")
    {
        string svg = await shareService.GenerateQrCode(link);
        throw ResponseFactory.Create<OkResponse<string>>(svg, ["image/svg+xml"]);
    }

    /// <summary>
    /// [TEST] Tests eager loading (Include/ThenInclude) with EF Core on distributor relationships.
    /// </summary>
    /// <returns>Distributor entity with loaded relationships.</returns>
    /// <response code="200">Entity retrieved successfully with loaded relationships.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify EF Core Include functionality.
    /// </remarks>
    [HttpGet("test_include")]
    [ProducesResponseType(typeof(OkResponse<Distributor>), StatusCodes.Status200OK)]
    public async Task<ActionResult> TestInclude()
    {
        Distributor? d = await distributorRepository.Include(d => d.Cover).Include(d => d.License)
            .ThenInclude(l => l.Issuer).GetByIdAsync(4);
        throw ResponseFactory.Create<OkResponse<Distributor>>(d);
    }

    /// <summary>
    /// [TEST] Checks application data directory existence and writes a test file.
    /// </summary>
    /// <returns>Success response if file written successfully.</returns>
    /// <response code="200">File written successfully to app data directory.</response>
    /// <response code="404">App data directory not found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify file system access.
    /// Used for testing Docker volume mounts.
    /// </remarks>
    [HttpGet("appdata")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// [TEST] Tests file upload functionality.
    /// </summary>
    /// <param name="file">The file to upload for testing.</param>
    /// <returns>Success response upon file upload.</returns>
    /// <response code="200">File uploaded successfully.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify file upload functionality.
    /// </remarks>
    [HttpPost("fileuploadtest")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> FileUploadTest(IFormFile file)
    {
        await fileService.UploadFileAsync(file);
        return Ok();
    }

    # endregion

    # region response tests

    /// <summary>
    /// [TEST] Generates a BadRequest error response for testing error handling.
    /// </summary>
    /// <returns>Bad request response.</returns>
    /// <response code="400">Test bad request error.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify error response handling.
    /// </remarks>
    [HttpGet("error")]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GenerateError()
    {
        throw ResponseFactory.Create<BadRequestResponse>(["Test exception"]);
    }

    /// <summary>
    /// [TEST] Generates a successful OK response for testing.
    /// </summary>
    /// <returns>OK response.</returns>
    /// <response code="200">Test OK response.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify success response handling.
    /// </remarks>
    [HttpGet("ok")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> GenerateOk()
    {
        throw ResponseFactory.Create<OkResponse>(["Test exception"]);
    }

    /// <summary>
    /// [TEST] Generates a successful OK response with sample data for testing.
    /// </summary>
    /// <returns>OK response with sample message data.</returns>
    /// <response code="200">Test OK response with data.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify data serialization in responses.
    /// </remarks>
    [HttpGet("okdata")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<Message>>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// [TEST] Retrieves settings by ID for testing purposes.
    /// </summary>
    /// <param name="settingsId">The ID of the settings to retrieve (default: 1).</param>
    /// <returns>Settings entity with full details.</returns>
    /// <response code="200">Settings retrieved successfully.</response>
    /// <response code="404">Settings not found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// </remarks>
    [HttpGet("settings/{settingsId?}")]
    [ProducesResponseType(typeof(OkResponse<Settings>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetSettings(int settingsId = 1)
    {
        throw ResponseFactory.Create<OkResponse<Settings>>(await settingsService.GetByIdValidatedFullAsync(settingsId),
            ["Settings retrieved successfully"]);
    }


    /// <summary>
    /// [TEST] Partially updates settings using JSON Patch for testing purposes.
    /// </summary>
    /// <param name="settingsId">The ID of the settings to update.</param>
    /// <param name="updates">JSON element containing the properties to update.</param>
    /// <returns>Success response upon settings update.</returns>
    /// <response code="200">Settings patched successfully.</response>
    /// <response code="404">Settings not found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify JSON Patch functionality.
    /// </remarks>
    [HttpPatch("settings/{settingsId:int}")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchSettings(int settingsId, [FromBody] JsonElement updates)
    {
        await settingsService.PatchAsync(settingsId, updates);
        throw ResponseFactory.Create<OkResponse>(["Settings patched successfully"]);
    }

    # endregion

    # region folder tests

    /// <summary>
    /// [TEST] Retrieves a folder by ID for testing purposes.
    /// </summary>
    /// <param name="folderId">The ID of the folder to retrieve.</param>
    /// <returns>Folder entity with details.</returns>
    /// <response code="200">Folder retrieved successfully.</response>
    /// <response code="404">Folder not found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// </remarks>
    [HttpGet("folder/{folderId:int}")]
    [ProducesResponseType(typeof(OkResponse<Folder>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFolder(int folderId)
    {
        Folder folder = await folderService.GetByIdValidatedAsync(folderId);
        throw ResponseFactory.Create<OkResponse<Folder>>(folder, ["Successfully retrieved folder"]);
    }

    /// <summary>
    /// [TEST] Retrieves all folders for testing purposes.
    /// </summary>
    /// <returns>Collection of folder entities.</returns>
    /// <response code="200">Folders retrieved successfully.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// </remarks>
    [HttpGet("folder")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<Folder>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFolders()
    {
        IEnumerable<Folder> folders = await folderService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<Folder>>>(folders, ["Successfully retrieved all folders"]);
    }

    /// <summary>
    /// [TEST] Creates a test folder with a timestamp-based name.
    /// </summary>
    /// <returns>Created folder entity.</returns>
    /// <response code="200">Folder created successfully.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// Creates a folder with name format "Test Folder{timestamp}".
    /// </remarks>
    [HttpPost("folder")]
    [ProducesResponseType(typeof(OkResponse<Folder>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// [TEST] Creates a default library structure for testing purposes.
    /// </summary>
    /// <returns>Created library entity with default folder structure.</returns>
    /// <response code="200">Default library created successfully.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// Creates a new library with the default folder structure.
    /// </remarks>
    [HttpPost("library/default")]
    [ProducesResponseType(typeof(OkResponse<Entities.Models.Library.Library>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateDefaultFolder()
    {
        throw ResponseFactory.Create<OkResponse<Entities.Models.Library.Library>>(
            await libraryService.CreateDefaultAsync(), ["Successfully created default folder"]);
    }

    # endregion
}