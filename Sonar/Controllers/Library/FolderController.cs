using Application;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Library;
using Application.Response;
using Entities.Models.Library;
using Entities.Models.Music;
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
    /// <response code="401">User not authenticated.</response>
    [HttpGet("{folderId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<FolderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFolder(int folderId)
    {
        User user = await CheckAccessFeatures([]);
        int libraryId = user.LibraryId;
        int userId = user.Id;
        Folder folder = await folderService.GetFolderByIdIncludeCollectionsValidatedAsync(folderId, libraryId);
        FolderDTO dto = new()
        {
            Id = folder.Id,
            Name = folder.Name,
            IsProtected = folder.IsProtected,
            ParentFolderId = folder.ParentFolderId,
            ParentFolderName = folder.ParentFolder?.Name,
            SubFolders = folder.SubFolders?.Select(sf => new FolderDTO
            {
                Id = sf.Id,
                Name = sf.Name,
                IsProtected = sf.IsProtected,
                ParentFolderId = sf.ParentFolderId,
                ParentFolderName = sf.ParentFolder?.Name,
                SubFolders = new List<FolderDTO>(),
                Collections = sf.Collections?
                    .Where(c =>
                    {
                        if (sf.IsProtected) return true;
                        if (c.VisibilityState == null) return false;
                        IEnumerable<int>? authorIds = c switch
                        {
                            Playlist playlist => playlist.CreatorId == userId ? [playlist.CreatorId] : null,
                            Album album => album.AlbumArtists?
                                .Where(aa => aa.Artist?.UserId != null)
                                .Select(aa => aa.Artist!.UserId!)
                                .ToList(),
                            _ => null
                        };
                        return VisibilityStateValidator.IsAccessible(c.VisibilityState, userId, authorIds);
                    })
                    .Select(c => new CollectionSummaryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = c.GetType().Name,
                        CoverId = c.CoverId
                    }).ToList() ?? new List<CollectionSummaryDTO>()
            }).ToList() ?? new List<FolderDTO>(),
            Collections = folder.Collections?
                .Where(c =>
                {
                    // Для root папки (IsProtected = true) показываем все коллекции (включая Favorites)
                    // Для остальных папок применяем фильтр по видимости
                    if (folder.IsProtected) return true;
                    if (c.VisibilityState == null) return false;
                    IEnumerable<int>? authorIds = c switch
                    {
                        Playlist playlist => playlist.CreatorId == userId ? [playlist.CreatorId] : null,
                        Album album => album.AlbumArtists?
                            .Where(aa => aa.Artist?.UserId != null)
                            .Select(aa => aa.Artist!.UserId!)
                            .ToList(),
                        _ => null
                    };
                    return VisibilityStateValidator.IsAccessible(c.VisibilityState, userId, authorIds);
                })
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
    /// Retrieves all folders in the user's library organized in a hierarchical structure.
    /// </summary>
    /// <returns>Collection of root folder DTOs with nested subfolders and collections.</returns>
    /// <response code="200">All folders retrieved successfully in hierarchical structure.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<FolderDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllFolders()
    {
        User user = await CheckAccessFeatures([]);
        int libraryId = user.LibraryId;
        int userId = user.Id;
        IEnumerable<Folder> folders = await folderService.GetAllFoldersWithCollectionsByLibraryIdAsync(libraryId);

        // Convert all folders to DTOs
        var folderDict = folders.ToDictionary(f => f.Id, f => new FolderDTO
        {
            Id = f.Id,
            Name = f.Name,
            IsProtected = f.IsProtected,
            ParentFolderId = f.ParentFolderId,
            ParentFolderName = f.ParentFolder?.Name,
            SubFolders = new List<FolderDTO>(),
            Collections = f.Collections?
                .Where(c =>
                {
                    // Для root папки (IsProtected = true) показываем все коллекции (включая Favorites)
                    // Для остальных папок применяем фильтр по видимости
                    if (f.IsProtected) return true;
                    if (c.VisibilityState == null) return false;
                    IEnumerable<int>? authorIds = c switch
                    {
                        Playlist playlist => playlist.CreatorId == userId ? [playlist.CreatorId] : null,
                        Album album => album.AlbumArtists?
                            .Where(aa => aa.Artist?.UserId != null)
                            .Select(aa => aa.Artist!.UserId!)
                            .ToList(),
                        _ => null
                    };
                    return VisibilityStateValidator.IsAccessible(c.VisibilityState, userId, authorIds);
                })
                .Select(c => new CollectionSummaryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.GetType().Name,
                    CoverId = c.CoverId
                }).ToList() ?? new List<CollectionSummaryDTO>()
        });

        // Build hierarchy: add subfolders to their parents
        foreach (var folder in folders)
        {
            if (folder.ParentFolderId.HasValue && folderDict.TryGetValue(folder.ParentFolderId.Value, out var parentDto))
            {
                if (folderDict.TryGetValue(folder.Id, out var childDto))
                {
                    parentDto.SubFolders.Add(childDto);
                }
            }
        }

        // Return only root folders (folders without parent)
        var rootFolders = folderDict.Values.Where(f => !f.ParentFolderId.HasValue).ToList();

        throw ResponseFactory.Create<OkResponse<IEnumerable<FolderDTO>>>(rootFolders, ["Successfully retrieved all folders in hierarchical structure"]);
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
            SubFolders = new List<FolderDTO>(),
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
        User user = await CheckAccessFeatures([]);
        int libraryId = user.LibraryId;
        Folder folder = await folderService.UpdateNameAsync(libraryId, folderId, newName);
        FolderDTO dto = new()
        {
            Id = folder.Id,
            Name = folder.Name,
            IsProtected = folder.IsProtected,
            ParentFolderId = folder.ParentFolderId,
            ParentFolderName = folder.ParentFolder?.Name,
            SubFolders = folder.SubFolders?.Select(sf => new FolderDTO
            {
                Id = sf.Id,
                Name = sf.Name,
                IsProtected = sf.IsProtected,
                ParentFolderId = sf.ParentFolderId,
                ParentFolderName = sf.ParentFolder?.Name,
                SubFolders = new List<FolderDTO>(),
                Collections = sf.Collections?
                    .Where(c =>
                    {
                        if (sf.IsProtected) return true;
                        if (c.VisibilityState == null) return false;
                        IEnumerable<int>? authorIds = c switch
                        {
                            Playlist playlist => playlist.CreatorId == user.Id ? [playlist.CreatorId] : null,
                            Album album => album.AlbumArtists?
                                .Where(aa => aa.Artist?.UserId != null)
                                .Select(aa => aa.Artist!.UserId!)
                                .ToList(),
                            _ => null
                        };
                        return VisibilityStateValidator.IsAccessible(c.VisibilityState, user.Id, authorIds);
                    })
                    .Select(c => new CollectionSummaryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = c.GetType().Name,
                        CoverId = c.CoverId
                    }).ToList() ?? new List<CollectionSummaryDTO>()
            }).ToList() ?? new List<FolderDTO>(),
            Collections = folder.Collections?
                .Where(c =>
                {
                    // Для root папки (IsProtected = true) показываем все коллекции (включая Favorites)
                    // Для остальных папок применяем фильтр по видимости
                    if (folder.IsProtected) return true;
                    if (c.VisibilityState == null) return false;
                    IEnumerable<int>? authorIds = c switch
                    {
                        Playlist playlist => playlist.CreatorId == user.Id ? [playlist.CreatorId] : null,
                        Album album => album.AlbumArtists?
                            .Where(aa => aa.Artist?.UserId != null)
                            .Select(aa => aa.Artist!.UserId!)
                            .ToList(),
                        _ => null
                    };
                    return VisibilityStateValidator.IsAccessible(c.VisibilityState, user.Id, authorIds);
                })
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
    /// <response code="200">Collection added to folder successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Folder or collection not found.</response>
    /// <response code="409">Collection is already in this folder.</response>
    [HttpPost("{folderId:int}/add-collection/{collectionId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCollectionToFolder(int folderId, int collectionId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.AddCollectionToFolderAsync(libraryId, folderId, collectionId);
        throw ResponseFactory.Create<OkResponse>(["Collection was added to folder successfully"]);
    }

    /// <summary>
    /// Removes a collection from a folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder.</param>
    /// <param name="collectionId">The ID of the collection to remove.</param>
    /// <returns>Success response upon removal.</returns>
    /// <response code="200">Collection removed from folder successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Cannot remove collections from protected system folders.</response>
    /// <response code="404">Folder or collection not found in this folder.</response>
    [HttpDelete("{folderId:int}/remove-collection/{collectionId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCollectionFromFolder(int folderId, int collectionId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.RemoveCollectionFromFolderAsync(libraryId, folderId, collectionId);
        throw ResponseFactory.Create<OkResponse>(["Collection was removed from folder successfully"]);
    }

    /// <summary>
    /// Moves a collection to a target folder, removing it from all other folders in the library (except protected folders).
    /// </summary>
    /// <param name="collectionId">The ID of the collection to move.</param>
    /// <param name="targetFolderId">The ID of the target folder.</param>
    /// <returns>Success response upon moving collection.</returns>
    /// <response code="200">Collection moved to folder successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Collection or target folder not found.</response>
    [HttpPut("move-collection/{collectionId:int}/to-folder/{targetFolderId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveCollectionToFolder(int collectionId, int targetFolderId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.MoveCollectionToFolderAsync(libraryId, collectionId, targetFolderId);
        throw ResponseFactory.Create<OkResponse>(["Collection was moved to folder successfully"]);
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