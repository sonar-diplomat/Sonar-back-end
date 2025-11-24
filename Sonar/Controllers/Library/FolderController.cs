using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Library;
using Application.Extensions;
using Application.Response;
using Entities.Models.Library;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Library;

[Route("api/[controller]")]
[ApiController]
public class FolderController(
    UserManager<User> userManager,
    IFolderService folderService,
    ILibraryService libraryService
) : BaseController(userManager)
{
    /// <summary>
    /// Retrieves a specific folder by ID with its subfolders and collections.
    /// </summary>
    /// <param name="folderId">The ID of the folder to retrieve.</param>
    /// <returns>Folder DTO with subfolders and collection summaries.</returns>
    /// <response code="200">Folder retrieved successfully.</response>
    /// <response code="404">Folder not found.</response>
    /// <remarks>
    /// TODO: Make it work only with a user from JWT for authorization.
    /// </remarks>
    [HttpGet("{folderId:int}")]
    [ProducesResponseType(typeof(OkResponse<FolderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFolder(int folderId)
    {
        Folder folder = await folderService.GetFolderByIdIncludeCollectionsValidatedAsync(folderId);
        FolderDTO dto = new()
        {
            Id = folder.Id,
            Name = folder.Name,
            IsProtected = folder.IsProtected,
            ParentFolderId = folder.ParentFolderId,
            ParentFolderName = folder.ParentFolder?.Name,
            SubFolders = folder.SubFolders?.Select(sf => new SubFolderDTO
            {
                Id = sf.Id,
                Name = sf.Name,
                IsProtected = sf.IsProtected,
                SubFolderCount = sf.SubFolders?.Count ?? 0,
                CollectionCount = sf.Collections?.Count ?? 0
            }).ToList() ?? new List<SubFolderDTO>(),
            Collections = folder.Collections?
                .Where(c => c.VisibilityState?.IsAccessible() ?? false)
                .Select(c => new CollectionSummaryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.GetType().Name,
                    CoverId = c.CoverId
                }).ToList() ?? new List<CollectionSummaryDTO>()
        };
        throw ResponseFactory.Create<OkResponse<FolderDTO>>(dto, ["Successfully retrieved folder"]);
    }

    /// <summary>
    /// Retrieves all folders in the system with their subfolders and collections.
    /// </summary>
    /// <returns>Collection of folder DTOs with subfolder and collection summaries.</returns>
    /// <response code="200">All folders retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<FolderDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFolders()
    {
        IEnumerable<Folder> folders = await folderService.GetAllFoldersWithCollectionsAsync();
        IEnumerable<FolderDTO> dtos = folders.Select(folder => new FolderDTO
        {
            Id = folder.Id,
            Name = folder.Name,
            IsProtected = folder.IsProtected,
            ParentFolderId = folder.ParentFolderId,
            ParentFolderName = folder.ParentFolder?.Name,
            SubFolders = folder.SubFolders?.Select(sf => new SubFolderDTO
            {
                Id = sf.Id,
                Name = sf.Name,
                IsProtected = sf.IsProtected,
                SubFolderCount = sf.SubFolders?.Count ?? 0,
                CollectionCount = sf.Collections?.Count ?? 0
            }).ToList() ?? new List<SubFolderDTO>(),
            Collections = folder.Collections?
                .Where(c => c.VisibilityState?.IsAccessible() ?? false)
                .Select(c => new CollectionSummaryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.GetType().Name,
                    CoverId = c.CoverId
                }).ToList() ?? new List<CollectionSummaryDTO>()
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<FolderDTO>>>(dtos, ["Successfully retrieved all folders"]);
    }

    /// <summary>
    /// Creates a new folder in the user's library.
    /// </summary>
    /// <param name="request">Create folder DTO with folder name and optional parent folder ID.</param>
    /// <returns>Created folder DTO.</returns>
    /// <response code="200">Folder created successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="400">Invalid folder data or parent folder not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<FolderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderDTO request)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        Folder folder = await folderService.CreateFolderAsync(libraryId, request);
        FolderDTO dto = new()
        {
            Id = folder.Id,
            Name = folder.Name,
            IsProtected = folder.IsProtected,
            ParentFolderId = folder.ParentFolderId,
            ParentFolderName = folder.ParentFolder?.Name,
            SubFolders = new List<SubFolderDTO>(),
            Collections = new List<CollectionSummaryDTO>()
        };
        throw ResponseFactory.Create<OkResponse<FolderDTO>>(dto, ["Successfully created folder"]);
    }

    /// <summary>
    /// Updates the name of an existing folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to rename.</param>
    /// <param name="newName">The new name for the folder.</param>
    /// <returns>Updated folder DTO.</returns>
    /// <response code="200">Folder name updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Cannot rename protected system folders.</response>
    /// <response code="404">Folder not found.</response>
    [HttpPut("{folderId:int}/update-name")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<FolderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFolderName(int folderId, [FromBody] string newName)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        Folder folder = await folderService.UpdateNameAsync(libraryId, folderId, newName);
        FolderDTO dto = new()
        {
            Id = folder.Id,
            Name = folder.Name,
            IsProtected = folder.IsProtected,
            ParentFolderId = folder.ParentFolderId,
            ParentFolderName = folder.ParentFolder?.Name,
            SubFolders = folder.SubFolders?.Select(sf => new SubFolderDTO
            {
                Id = sf.Id,
                Name = sf.Name,
                IsProtected = sf.IsProtected,
                SubFolderCount = sf.SubFolders?.Count ?? 0,
                CollectionCount = sf.Collections?.Count ?? 0
            }).ToList() ?? new List<SubFolderDTO>(),
            Collections = folder.Collections?
                .Where(c => c.VisibilityState?.IsAccessible() ?? false)
                .Select(c => new CollectionSummaryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.GetType().Name,
                    CoverId = c.CoverId
                }).ToList() ?? new List<CollectionSummaryDTO>()
        };
        throw ResponseFactory.Create<OkResponse<FolderDTO>>(dto, ["Successfully updated folder name"]);
    }

    /// <summary>
    /// Deletes a folder from the user's library.
    /// </summary>
    /// <param name="folderId">The ID of the folder to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Folder deleted successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Cannot delete protected system folders.</response>
    /// <response code="404">Folder not found.</response>
    [HttpDelete("{folderId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFolder(int folderId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.DeleteFolderAsync(libraryId, folderId);
        throw ResponseFactory.Create<OkResponse>(["Successfully deleted folder"]);
    }

    /// <summary>
    /// Adds a collection (album, playlist, blend) to a folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder.</param>
    /// <param name="collectionId">The ID of the collection to add.</param>
    /// <returns>Success response upon addition.</returns>
    /// <response code="501">Not yet implemented.</response>
    /// <remarks>
    /// TODO: Implement adding/removing collections to/from folders when collections have been tested.
    /// </remarks>
    [HttpPost("{folderId:int}/add-collection/{collectionId:int}")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> AddCollectionToFolder(int folderId, int collectionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a collection from a folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder.</param>
    /// <param name="collectionId">The ID of the collection to remove.</param>
    /// <returns>Success response upon removal.</returns>
    /// <response code="501">Not yet implemented.</response>
    [HttpDelete("{folderId:int}/remove-collection/{collectionId:int}")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> RemoveCollectionFromFolder(int folderId, int collectionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Moves a folder to a new parent folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to move.</param>
    /// <param name="newParentFolderId">The ID of the new parent folder.</param>
    /// <returns>Success response upon folder move.</returns>
    /// <response code="200">Folder moved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="400">Cannot move folder into itself or its descendants.</response>
    /// <response code="403">Cannot move protected system folders.</response>
    /// <response code="404">Folder or parent folder not found.</response>
    [HttpPut("{folderId:int}/move/{newParentFolderId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveFolder(int folderId, int newParentFolderId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.MoveFolder(libraryId, folderId, newParentFolderId);
        throw ResponseFactory.Create<OkResponse>(["Folder was successfully moved"]);
    }
}