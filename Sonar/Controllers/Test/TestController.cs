using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Extensions;
using Application.Response;
using Entities.Enums;
using Entities.Models.Chat;
using Entities.Models.Distribution;
using Entities.Models.File;
using Entities.Models.Library;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    IImageFileService imageFileService,
    ITrackRepository trackRepository,
    SonarContext dbContext,
    IFileStorageService fileStorageService,
    IAccessFeatureService accessFeatureService,
    IEmailSenderService emailSenderService,
    Analytics.API.Analytics.AnalyticsClient analyticsClient,
    Application.Abstractions.Interfaces.Repository.Music.IArtistRepository artistRepository
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
    /// [TEST] Tests eager loading (SnInclude/SnThenInclude) with EF Core on distributor relationships.
    /// </summary>
    /// <returns>Distributor entity with loaded relationships.</returns>
    /// <response code="200">Entity retrieved successfully with loaded relationships.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify EF Core SnInclude functionality.
    /// </remarks>
    [HttpGet("test_include")]
    [ProducesResponseType(typeof(OkResponse<Distributor>), StatusCodes.Status200OK)]
    public async Task<ActionResult> TestInclude()
    {
        Distributor? d = await distributorRepository.SnInclude(d => d.Cover).SnInclude(d => d.License)
            .SnThenInclude(l => l.Issuer).GetByIdAsync(4);
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

    /// <summary>
    /// [TEST] Uploads an image file without binding to any specific entity.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>Uploaded image file with its ID.</returns>
    /// <response code="200">Image file uploaded successfully.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// Uploads an image file and returns the created ImageFile entity.
    /// The file can be later bound to any entity by updating its ID reference.
    /// </remarks>
    [HttpPost("upload-image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(OkResponse<ImageFile>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        ImageFile uploadedImage = await imageFileService.UploadFileAsync(file);
        throw ResponseFactory.Create<OkResponse<ImageFile>>(uploadedImage, ["Image file uploaded successfully"]);
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

    # region log tests

    /// <summary>
    /// [TEST] Retrieves the most recent log file.
    /// </summary>
    /// <returns>The most recent log file content and path.</returns>
    /// <response code="200">Log file retrieved successfully.</response>
    /// <response code="404">No log files found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// Returns the most recently written log file from both general and guild logs directories.
    /// </remarks>
    [HttpGet("logs/last")]
    [ProducesResponseType(typeof(OkResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetLastLogFile()
    {
        var result = await Logging.Logger.GetLastLogFileAsync();
        if (result == null)
            throw ResponseFactory.Create<NotFoundResponse>(["No log files found"]);

        var response = new
        {
            FilePath = result.Value.FilePath,
            Content = System.Text.Json.JsonSerializer.Deserialize<object>(result.Value.Content)
        };

        throw ResponseFactory.Create<OkResponse<object>>(response, ["Last log file retrieved successfully"]);
    }

    [HttpGet("logs/category/{category}")]
    [ProducesResponseType(typeof(OkResponse<List<LogEntry>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetCategoryLogs(
        LogCategory category,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] LogLevel? minLevel = null,
        [FromQuery] LogLevel? maxLevel = null,
        [FromQuery] LogLevel? exactLevel = null,
        [FromQuery] int? limit = null)
    {
        List<LogEntry> logs = await Logging.Logger.GetCategoryLogsAsync(
            category,
            fromDate,
            toDate,
            minLevel,
            maxLevel,
            exactLevel,
            limit);

        throw ResponseFactory.Create<OkResponse<List<LogEntry>>>(logs, ["Category logs retrieved successfully"]);
    }

    [HttpGet("logs/guild/{guildId}")]
    [ProducesResponseType(typeof(OkResponse<List<LogEntry>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetGuildLogs(
        ulong guildId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] LogLevel? minLevel = null,
        [FromQuery] LogLevel? maxLevel = null,
        [FromQuery] LogLevel? exactLevel = null,
        [FromQuery] int? limit = null)
    {
        List<LogEntry> logs = await Logging.Logger.GetGuildLogsAsync(
            guildId,
            null,
            fromDate,
            toDate,
            minLevel,
            maxLevel,
            exactLevel,
            limit);

        throw ResponseFactory.Create<OkResponse<List<LogEntry>>>(logs, ["Guild logs retrieved successfully"]);
    }

    # endregion

    # region file validation tests

    /// <summary>
    /// [TEST] Validates all track files and finds orphaned files.
    /// </summary>
    /// <returns>Information about track files and orphaned files.</returns>
    /// <response code="200">File validation completed successfully.</response>
    /// <remarks>
    /// This endpoint iterates through all tracks, checks if their files exist,
    /// and also returns files that are not referenced by any track or image.
    /// </remarks>
    [HttpGet("files/validate")]
    [ProducesResponseType(typeof(OkResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateTrackFiles()
    {
        string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        // Get all tracks with their files
        IQueryable<Entities.Models.Music.Track> tracksQuery = await trackRepository.GetAllAsync();
        List<Entities.Models.Music.Track> tracks = await tracksQuery
            .Include(t => t.LowQualityAudioFile)
            .Include(t => t.MediumQualityAudioFile)
            .Include(t => t.HighQualityAudioFile)
            .Include(t => t.Cover)
            .ToListAsync();

        // Get all referenced file IDs
        HashSet<int> referencedAudioFileIds = new();
        HashSet<int> referencedImageFileIds = new();

        var trackFileInfo = new List<object>();

        foreach (var track in tracks)
        {
            var files = new List<object>();

            // Check LowQualityAudioFile
            if (track.LowQualityAudioFile != null)
            {
                referencedAudioFileIds.Add(track.LowQualityAudioFile.Id);
                string physicalPath = GetPhysicalPath(track.LowQualityAudioFile.Url, baseFolder);
                bool exists = SysFile.Exists(physicalPath);
                files.Add(new
                {
                    Type = "LowQualityAudio",
                    FileId = track.LowQualityAudioFile.Id,
                    FileName = track.LowQualityAudioFile.ItemName,
                    Url = track.LowQualityAudioFile.Url,
                    PhysicalPath = physicalPath,
                    Exists = exists,
                    FileSize = exists ? new FileInfo(physicalPath).Length : (long?)null
                });
            }

            // Check MediumQualityAudioFile
            if (track.MediumQualityAudioFile != null)
            {
                referencedAudioFileIds.Add(track.MediumQualityAudioFile.Id);
                string physicalPath = GetPhysicalPath(track.MediumQualityAudioFile.Url, baseFolder);
                bool exists = SysFile.Exists(physicalPath);
                files.Add(new
                {
                    Type = "MediumQualityAudio",
                    FileId = track.MediumQualityAudioFile.Id,
                    FileName = track.MediumQualityAudioFile.ItemName,
                    Url = track.MediumQualityAudioFile.Url,
                    PhysicalPath = physicalPath,
                    Exists = exists,
                    FileSize = exists ? new FileInfo(physicalPath).Length : (long?)null
                });
            }

            // Check HighQualityAudioFile
            if (track.HighQualityAudioFile != null)
            {
                referencedAudioFileIds.Add(track.HighQualityAudioFile.Id);
                string physicalPath = GetPhysicalPath(track.HighQualityAudioFile.Url, baseFolder);
                bool exists = SysFile.Exists(physicalPath);
                files.Add(new
                {
                    Type = "HighQualityAudio",
                    FileId = track.HighQualityAudioFile.Id,
                    FileName = track.HighQualityAudioFile.ItemName,
                    Url = track.HighQualityAudioFile.Url,
                    PhysicalPath = physicalPath,
                    Exists = exists,
                    FileSize = exists ? new FileInfo(physicalPath).Length : (long?)null
                });
            }

            // Check Cover
            if (track.Cover != null)
            {
                referencedImageFileIds.Add(track.Cover.Id);
                string physicalPath = GetPhysicalPath(track.Cover.Url, baseFolder);
                bool exists = SysFile.Exists(physicalPath);
                files.Add(new
                {
                    Type = "Cover",
                    FileId = track.Cover.Id,
                    FileName = track.Cover.ItemName,
                    Url = track.Cover.Url,
                    PhysicalPath = physicalPath,
                    Exists = exists,
                    FileSize = exists ? new FileInfo(physicalPath).Length : (long?)null
                });
            }

            trackFileInfo.Add(new
            {
                TrackId = track.Id,
                TrackTitle = track.Title,
                Files = files
            });
        }

        // Get image files referenced by Users, Albums, Distributors, Chats
        var userImageIds = await dbContext.Users.Select(u => u.AvatarImageId).ToListAsync();
        var albumImageIds = await dbContext.Albums.Select(a => a.CoverId).ToListAsync();
        var distributorImageIds = await dbContext.Distributors.Select(d => d.CoverId).ToListAsync();
        var chatImageIds = await dbContext.Chats.Select(c => c.CoverId).ToListAsync();

        referencedImageFileIds.UnionWith(userImageIds);
        referencedImageFileIds.UnionWith(albumImageIds);
        referencedImageFileIds.UnionWith(distributorImageIds);
        referencedImageFileIds.UnionWith(chatImageIds);

        // Find orphaned files in database
        var orphanedAudioFiles = await dbContext.AudioFiles
            .Where(f => !referencedAudioFileIds.Contains(f.Id))
            .Select(f => new { f.Id, f.ItemName, f.Url })
            .ToListAsync();

        var orphanedImageFiles = await dbContext.ImageFiles
            .Where(f => !referencedImageFileIds.Contains(f.Id))
            .Select(f => new { f.Id, f.ItemName, f.Url })
            .ToListAsync();

        // Find orphaned files on disk
        var orphanedDiskFiles = new List<object>();

        if (Directory.Exists(baseFolder))
        {
            var allDiskFiles = Directory.GetFiles(baseFolder, "*", SearchOption.AllDirectories);
            var allReferencedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Get all referenced URLs from database
            var allReferencedAudioUrls = await dbContext.AudioFiles
                .Where(f => referencedAudioFileIds.Contains(f.Id))
                .Select(f => f.Url)
                .ToListAsync();

            var allReferencedImageUrls = await dbContext.ImageFiles
                .Where(f => referencedImageFileIds.Contains(f.Id))
                .Select(f => f.Url)
                .ToListAsync();

            allReferencedUrls.UnionWith(allReferencedAudioUrls);
            allReferencedUrls.UnionWith(allReferencedImageUrls);

            foreach (var diskFile in allDiskFiles)
            {
                string relativePath = Path.GetRelativePath(baseFolder, diskFile)
                    .Replace('\\', '/');

                // Check if this file is referenced
                bool isReferenced = allReferencedUrls.Contains(relativePath);

                if (!isReferenced)
                {
                    var fileInfo = new FileInfo(diskFile);
                    orphanedDiskFiles.Add(new
                    {
                        RelativePath = relativePath,
                        PhysicalPath = diskFile,
                        FileSize = fileInfo.Length,
                        LastModified = fileInfo.LastWriteTime
                    });
                }
            }
        }

        var result = new
        {
            TrackFiles = trackFileInfo,
            OrphanedDatabaseFiles = new
            {
                AudioFiles = orphanedAudioFiles,
                ImageFiles = orphanedImageFiles
            },
            OrphanedDiskFiles = orphanedDiskFiles,
            Summary = new
            {
                TotalTracks = tracks.Count,
                TotalReferencedAudioFiles = referencedAudioFileIds.Count,
                TotalReferencedImageFiles = referencedImageFileIds.Count,
                OrphanedAudioFilesCount = orphanedAudioFiles.Count,
                OrphanedImageFilesCount = orphanedImageFiles.Count,
                OrphanedDiskFilesCount = orphanedDiskFiles.Count
            }
        };

        throw ResponseFactory.Create<OkResponse<object>>(result, ["File validation completed successfully"]);
    }

    private string GetPhysicalPath(string relativePath, string baseFolder)
    {
        string normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar)
                                           .Replace('\\', Path.DirectorySeparatorChar)
                                           .TrimStart('/', '\\');
        return Path.Combine(baseFolder, normalizedPath);
    }

    /// <summary>
    /// [TEST] Deletes orphaned files from database and disk.
    /// </summary>
    /// <returns>Information about deleted files.</returns>
    /// <response code="200">Files deleted successfully.</response>
    /// <remarks>
    /// This endpoint checks all files (AudioFile, ImageFile, VideoFile) for references.
    /// If a file is not referenced anywhere, it will be deleted from the database
    /// and an attempt will be made to delete it from disk.
    /// </remarks>
    [HttpPost("files/cleanup")]
    [ProducesResponseType(typeof(OkResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CleanupOrphanedFiles()
    {
        // Get all referenced file IDs
        var referencedAudioFileIds = new HashSet<int>();
        var referencedImageFileIds = new HashSet<int>();
        var referencedVideoFileIds = new HashSet<int>();

        // Get AudioFile references from Tracks
        var lowQualityIds = await dbContext.Tracks
            .Select(t => t.LowQualityAudioFileId)
            .ToListAsync();

        var mediumQualityIds = await dbContext.Tracks
            .Where(t => t.MediumQualityAudioFileId != null)
            .Select(t => t.MediumQualityAudioFileId!.Value)
            .ToListAsync();

        var highQualityIds = await dbContext.Tracks
            .Where(t => t.HighQualityAudioFileId != null)
            .Select(t => t.HighQualityAudioFileId!.Value)
            .ToListAsync();

        referencedAudioFileIds.UnionWith(lowQualityIds);
        referencedAudioFileIds.UnionWith(mediumQualityIds);
        referencedAudioFileIds.UnionWith(highQualityIds);

        // Get ImageFile references
        var trackImageIds = await dbContext.Tracks.Select(t => t.CoverId).ToListAsync();
        var albumImageIds = await dbContext.Albums.Select(a => a.CoverId).ToListAsync();
        var distributorImageIds = await dbContext.Distributors.Select(d => d.CoverId).ToListAsync();
        var userImageIds = await dbContext.Users.Select(u => u.AvatarImageId).ToListAsync();
        var chatImageIds = await dbContext.Chats.Select(c => c.CoverId).ToListAsync();

        referencedImageFileIds.UnionWith(trackImageIds);
        referencedImageFileIds.UnionWith(albumImageIds);
        referencedImageFileIds.UnionWith(distributorImageIds);
        referencedImageFileIds.UnionWith(userImageIds);
        referencedImageFileIds.UnionWith(chatImageIds);

        // Find orphaned files
        var orphanedAudioFiles = await dbContext.AudioFiles
            .Where(f => !referencedAudioFileIds.Contains(f.Id))
            .ToListAsync();

        var orphanedImageFiles = await dbContext.ImageFiles
            .Where(f => !referencedImageFileIds.Contains(f.Id))
            .ToListAsync();

        var orphanedVideoFiles = await dbContext.VideoFiles
            .Where(f => !referencedVideoFileIds.Contains(f.Id))
            .ToListAsync();

        var deletedFiles = new List<object>();
        var deletionErrors = new List<object>();
        var skippedDueToForeignKey = new List<object>();
        int deletedCount = 0;

        // Helper method to check if exception is a foreign key constraint violation
        static bool IsForeignKeyConstraintError(Exception ex)
        {
            if (ex is DbUpdateException dbEx)
            {
                // Check for PostgreSQL foreign key error (code 23503)
                if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23503")
                    return true;

                // Check error message for foreign key references
                string errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                if (errorMessage.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                    errorMessage.Contains("violates foreign key constraint", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        // Delete orphaned AudioFiles
        foreach (var audioFile in orphanedAudioFiles)
        {
            try
            {
                // Try to delete from disk
                bool diskDeleted = await fileStorageService.DeleteFile(audioFile.Url);

                // Delete from database
                dbContext.AudioFiles.Remove(audioFile);

                // Save changes immediately for this file
                await dbContext.SaveChangesAsync();
                deletedCount++;

                deletedFiles.Add(new
                {
                    Type = "AudioFile",
                    FileId = audioFile.Id,
                    FileName = audioFile.ItemName,
                    Url = audioFile.Url,
                    DeletedFromDatabase = true,
                    DeletedFromDisk = diskDeleted
                });
            }
            catch (DbUpdateException dbEx) when (IsForeignKeyConstraintError(dbEx))
            {
                // Rollback the removal from context
                dbContext.Entry(audioFile).State = EntityState.Unchanged;

                skippedDueToForeignKey.Add(new
                {
                    Type = "AudioFile",
                    FileId = audioFile.Id,
                    FileName = audioFile.ItemName,
                    Url = audioFile.Url,
                    Reason = "File is referenced by foreign key constraint"
                });
            }
            catch (Exception ex)
            {
                // Rollback the removal from context if it was added
                var entry = dbContext.Entry(audioFile);
                if (entry.State == EntityState.Deleted)
                    entry.State = EntityState.Unchanged;

                deletionErrors.Add(new
                {
                    Type = "AudioFile",
                    FileId = audioFile.Id,
                    FileName = audioFile.ItemName,
                    Url = audioFile.Url,
                    Error = ex.Message
                });
            }
        }

        // Delete orphaned ImageFiles
        foreach (var imageFile in orphanedImageFiles)
        {
            try
            {
                // Try to delete from disk
                bool diskDeleted = await fileStorageService.DeleteFile(imageFile.Url);

                // Delete from database
                dbContext.ImageFiles.Remove(imageFile);

                // Save changes immediately for this file
                await dbContext.SaveChangesAsync();
                deletedCount++;

                deletedFiles.Add(new
                {
                    Type = "ImageFile",
                    FileId = imageFile.Id,
                    FileName = imageFile.ItemName,
                    Url = imageFile.Url,
                    DeletedFromDatabase = true,
                    DeletedFromDisk = diskDeleted
                });
            }
            catch (DbUpdateException dbEx) when (IsForeignKeyConstraintError(dbEx))
            {
                // Rollback the removal from context
                dbContext.Entry(imageFile).State = EntityState.Unchanged;

                skippedDueToForeignKey.Add(new
                {
                    Type = "ImageFile",
                    FileId = imageFile.Id,
                    FileName = imageFile.ItemName,
                    Url = imageFile.Url,
                    Reason = "File is referenced by foreign key constraint"
                });
            }
            catch (Exception ex)
            {
                // Rollback the removal from context if it was added
                var entry = dbContext.Entry(imageFile);
                if (entry.State == EntityState.Deleted)
                    entry.State = EntityState.Unchanged;

                deletionErrors.Add(new
                {
                    Type = "ImageFile",
                    FileId = imageFile.Id,
                    FileName = imageFile.ItemName,
                    Url = imageFile.Url,
                    Error = ex.Message
                });
            }
        }

        // Delete orphaned VideoFiles
        foreach (var videoFile in orphanedVideoFiles)
        {
            try
            {
                // Try to delete from disk
                bool diskDeleted = await fileStorageService.DeleteFile(videoFile.Url);

                // Delete from database
                dbContext.VideoFiles.Remove(videoFile);

                // Save changes immediately for this file
                await dbContext.SaveChangesAsync();
                deletedCount++;

                deletedFiles.Add(new
                {
                    Type = "VideoFile",
                    FileId = videoFile.Id,
                    FileName = videoFile.ItemName,
                    Url = videoFile.Url,
                    DeletedFromDatabase = true,
                    DeletedFromDisk = diskDeleted
                });
            }
            catch (DbUpdateException dbEx) when (IsForeignKeyConstraintError(dbEx))
            {
                // Rollback the removal from context
                dbContext.Entry(videoFile).State = EntityState.Unchanged;

                skippedDueToForeignKey.Add(new
                {
                    Type = "VideoFile",
                    FileId = videoFile.Id,
                    FileName = videoFile.ItemName,
                    Url = videoFile.Url,
                    Reason = "File is referenced by foreign key constraint"
                });
            }
            catch (Exception ex)
            {
                // Rollback the removal from context if it was added
                var entry = dbContext.Entry(videoFile);
                if (entry.State == EntityState.Deleted)
                    entry.State = EntityState.Unchanged;

                deletionErrors.Add(new
                {
                    Type = "VideoFile",
                    FileId = videoFile.Id,
                    FileName = videoFile.ItemName,
                    Url = videoFile.Url,
                    Error = ex.Message
                });
            }
        }

        var result = new
        {
            DeletedFiles = deletedFiles,
            DeletionErrors = deletionErrors,
            SkippedDueToForeignKey = skippedDueToForeignKey,
            Summary = new
            {
                TotalOrphanedAudioFiles = orphanedAudioFiles.Count,
                TotalOrphanedImageFiles = orphanedImageFiles.Count,
                TotalOrphanedVideoFiles = orphanedVideoFiles.Count,
                SuccessfullyDeleted = deletedFiles.Count,
                DeletionErrorsCount = deletionErrors.Count,
                SkippedDueToForeignKeyCount = skippedDueToForeignKey.Count,
                DatabaseRecordsDeleted = deletedCount
            }
        };

        throw ResponseFactory.Create<OkResponse<object>>(result, ["File cleanup completed"]);
    }

    /// <summary>
    /// [TEST] Updates the default image file (file with ID 1).
    /// </summary>
    /// <param name="file">The new image file to replace the default image.</param>
    /// <returns>Success response upon default image update.</returns>
    /// <response code="200">Default image updated successfully.</response>
    /// <response code="404">Default image file not found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// Replaces the default image file (ID = 1) with a new uploaded image.
    /// </remarks>
    [HttpPut("default-image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(OkResponse<ImageFile>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDefaultImage([FromForm] IFormFile file)
    {
        // Get the default image file (ID = 1)
        ImageFile? defaultImage = await dbContext.ImageFiles.FirstOrDefaultAsync(f => f.Id == 1);
        if (defaultImage == null)
        {
            throw ResponseFactory.Create<NotFoundResponse>(["Default image file (ID = 1) not found"]);
        }

        // Save the old URL before updating
        string oldUrl = defaultImage.Url;

        // Upload the new image file
        ImageFile newImage = await imageFileService.UploadFileAsync(file);

        // Update the default image file with new URL and filename
        defaultImage.Url = newImage.Url;
        defaultImage.ItemName = newImage.ItemName;

        // Save changes to database
        await dbContext.SaveChangesAsync();

        // Delete the old file from disk
        await fileStorageService.DeleteFile(oldUrl);

        // Delete the newly created image file record (we only needed its URL)
        dbContext.ImageFiles.Remove(newImage);
        await dbContext.SaveChangesAsync();

        throw ResponseFactory.Create<OkResponse<ImageFile>>(defaultImage, ["Default image updated successfully"]);
    }

    /// <summary>
    /// [TEST] Updates an existing image file by ID with a new uploaded image.
    /// </summary>
    /// <param name="imageId">The ID of the image file to update.</param>
    /// <param name="url">The new URL for the image file.</param>
    /// <param name="itemName">The new item name for the image file.</param>
    /// <returns>Updated image file.</returns>
    /// <response code="200">Image file updated successfully.</response>
    /// <response code="404">Image file not found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// Updates an existing image file record with new URL and item name.
    /// The actual file should be uploaded first using upload-image endpoint.
    /// </remarks>
    [HttpPut("image/{imageId}")]
    [ProducesResponseType(typeof(OkResponse<ImageFile>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateImageById(int imageId, [FromBody] UpdateImageRequest request)
    {
        ImageFile? imageFile = await dbContext.ImageFiles.FirstOrDefaultAsync(f => f.Id == imageId);
        if (imageFile == null)
        {
            throw ResponseFactory.Create<NotFoundResponse>([$"Image file with ID {imageId} not found"]);
        }

        string oldUrl = imageFile.Url;
        imageFile.Url = request.Url;
        imageFile.ItemName = request.ItemName;

        await dbContext.SaveChangesAsync();

        if (!string.IsNullOrEmpty(oldUrl) && oldUrl != request.Url)
        {
            await fileStorageService.DeleteFile(oldUrl);
        }

        throw ResponseFactory.Create<OkResponse<ImageFile>>(imageFile, ["Image file updated successfully"]);
    }

    public class UpdateImageRequest
    {
        public string Url { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
    }

    /// <summary>
    /// [TEST] Assigns IamAGod access feature to a user (bypasses normal protection).
    /// </summary>
    /// <param name="userId">The ID of the user to assign IamAGod to.</param>
    /// <returns>Success response upon assignment.</returns>
    /// <response code="200">IamAGod assigned successfully.</response>
    /// <response code="404">User or IamAGod feature not found.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes.
    /// Bypasses the normal protection that prevents assigning IamAGod through regular methods.
    /// </remarks>
    [HttpPost("assign-iamagod/{userId}")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignIamAGod(int userId)
    {
        User? user = await dbContext.Users
            .Include(u => u.AccessFeatures)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw ResponseFactory.Create<NotFoundResponse>([$"User with ID {userId} not found"]);
        }

        Entities.Models.Access.AccessFeature? iamAGod = await accessFeatureService.GetByNameValidatedAsync("IamAGod");

        if (user.AccessFeatures.Any(af => af.Id == iamAGod.Id))
        {
            throw ResponseFactory.Create<OkResponse>(["User already has IamAGod access feature"]);
        }

        user.AccessFeatures.Add(iamAGod);
        await dbContext.SaveChangesAsync();

        throw ResponseFactory.Create<OkResponse>(["IamAGod access feature assigned successfully"]);
    }

    # endregion

    # region email tests

    /// <summary>
    /// [TEST] Sends a test email using the specified template.
    /// </summary>
    /// <param name="template">Template name (2fa, confirm-email, recovery-password).</param>
    /// <param name="email">Email address to send the test email to.</param>
    /// <param name="variables">Optional variables for template replacement (e.g., {"code": "123456", "link": "https://example.com"}).</param>
    /// <returns>Success response upon email send.</returns>
    /// <response code="200">Test email sent successfully.</response>
    /// <response code="400">Invalid template name or email address.</response>
    /// <remarks>
    /// This is a test endpoint for development/testing purposes to verify email sending functionality.
    /// Available templates: "2fa", "confirm-email", "recovery-password"
    /// </remarks>
    [HttpPost("send-email")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTestEmail(
        [FromQuery] string template,
        [FromQuery] string email,
        [FromBody] Dictionary<string, string>? variables = null)
    {
        if (string.IsNullOrWhiteSpace(template))
            throw ResponseFactory.Create<BadRequestResponse>(["Template name is required"]);

        if (string.IsNullOrWhiteSpace(email))
            throw ResponseFactory.Create<BadRequestResponse>(["Email address is required"]);

        // Если переменные не переданы, создаем тестовые значения
        if (variables == null || variables.Count == 0)
        {
            variables = template switch
            {
                "2fa" => new Dictionary<string, string> { { "code", "123456" } },
                "confirm-email" => new Dictionary<string, string> { { "link", "https://example.com/confirm-email?email=test@example.com&token=test-token-123" } },
                "recovery-password" => new Dictionary<string, string> { { "link", "https://example.com/reset-password?email=test@example.com&token=test-token-456" } },
                _ => new Dictionary<string, string>()
            };
        }

        await emailSenderService.SendEmailAsync(email, template, variables);

        throw ResponseFactory.Create<OkResponse>([$"Test email sent successfully to {email} using template '{template}'"]);
    }

    /// <summary>
    /// Test endpoint to retrieve top tracks and artists for a random user.
    /// </summary>
    /// <returns>Combined response with top tracks and top artists data.</returns>
    /// <response code="200">Test data retrieved successfully.</response>
    [HttpGet("top-user-stats")]
    [ProducesResponseType(typeof(OkResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestTopUserStats()
    {
        // Get a random user from the database
        var users = await dbContext.Users.ToListAsync();
        if (!users.Any())
        {
            throw ResponseFactory.Create<BadRequestResponse>(["No users found in database"]);
        }

        var randomUser = users[new Random().Next(users.Count)];

        try
        {
            // TODO: Call analytics service to get top track IDs
            var topTracksRequest = new Analytics.API.GetTopTracksRequest
            {
                UserId = randomUser.Id,
                Limit = 5
            };
            var topTracksResponse = await analyticsClient.GetTopTracksAsync(topTracksRequest);

            // TODO: Call analytics service to get top artist IDs
            var topArtistsRequest = new Analytics.API.GetTopArtistsRequest
            {
                UserId = randomUser.Id,
                Limit = 5
            };
            var topArtistsResponse = await analyticsClient.GetTopArtistsAsync(topArtistsRequest);

            // Process top tracks
            var topTrackDtos = new List<object>();
            if (topTracksResponse.Tracks != null && topTracksResponse.Tracks.Any())
            {
                var trackIds = topTracksResponse.Tracks.Select(t => t.TrackId).ToList();
                var playCountsByTrackId = topTracksResponse.Tracks.ToDictionary(t => t.TrackId, t => t.PlayCount);

                var tracks = await trackRepository.GetAllAsync();
                var topTracks = await tracks
                    .Where(t => trackIds.Contains(t.Id))
                    .SnInclude(t => t.TrackArtists)
                    .ThenInclude(ta => ta.Artist)
                    .SnInclude(t => t.Collections)
                    .ToListAsync();

                topTrackDtos = topTracks.Select(t =>
                {
                    var album = t.Collections?.OfType<Entities.Models.Music.Album>().FirstOrDefault();
                    return new
                    {
                        Id = t.Id,
                        Title = t.Title,
                        DurationInSeconds = (int)(t.Duration?.TotalSeconds ?? 0),
                        CoverId = t.CoverId,
                        Artists = (t.TrackArtists?.Select(ta => new
                        {
                            Pseudonym = ta.Pseudonym,
                            ArtistId = ta.ArtistId
                        }) ?? Enumerable.Empty<object>()).ToList(),
                        AlbumId = album?.Id,
                        AlbumName = album?.Name,
                        PlayCount = playCountsByTrackId.GetValueOrDefault(t.Id, 0)
                    };
                })
                .OrderByDescending(t => t.PlayCount)
                .Cast<object>()
                .ToList();
            }

            // Process top artists
            var topArtistDtos = new List<object>();
            if (topArtistsResponse.Artists != null && topArtistsResponse.Artists.Any())
            {
                var artistIds = topArtistsResponse.Artists.Select(a => a.ArtistId).ToList();
                var playCountsByArtistId = topArtistsResponse.Artists.ToDictionary(a => a.ArtistId, a => a.PlayCount);

                var artists = await artistRepository.GetAllAsync();
                var topArtists = await artists
                    .Where(a => artistIds.Contains(a.Id))
                    .SnInclude(a => a.User)
                    .ToListAsync();

                topArtistDtos = topArtists.Select(a => new
                {
                    Id = a.Id,
                    ArtistName = a.ArtistName,
                    UserId = a.UserId,
                    AvatarImageId = a.User?.AvatarImageId,
                    PlayCount = playCountsByArtistId.GetValueOrDefault(a.Id, 0)
                })
                .OrderByDescending(a => a.PlayCount)
                .Cast<object>()
                .ToList();
            }

            var result = new
            {
                TestUser = new
                {
                    randomUser.Id,
                    randomUser.UserName,
                    randomUser.PublicIdentifier
                },
                TopTracks = topTrackDtos,
                TopArtists = topArtistDtos,
                Message = "Data retrieved from analytics service for random user"
            };

            throw ResponseFactory.Create<OkResponse<object>>(result, ["Top user stats test completed successfully"]);
        }
        catch (Exception ex)
        {
            throw ResponseFactory.Create<BadRequestResponse>([$"Error calling analytics service: {ex.Message}"]);
        }
    }

    # endregion
}