using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Library;
using Application.Response;
using Entities.Models.Library;
using Entities.Models.UserCore;
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
    // TODO: write XML comments and returnType attributes
    // TODO: Make it work only with a user from JWT
    [HttpGet("{folderId:int}")]
    public async Task<IActionResult> GetFolder(int folderId)
    {
        Folder folder = await folderService.GetByIdValidatedAsync(folderId);
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
            Collections = folder.Collections?.Select(c => new CollectionSummaryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.GetType().Name,
                CoverUrl = c.Cover?.Url ?? string.Empty
            }).ToList() ?? new List<CollectionSummaryDTO>()
        };
        throw ResponseFactory.Create<OkResponse<FolderDTO>>(dto, ["Successfully retrieved folder"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet]
    public async Task<IActionResult> GetAllFolders()
    {
        IEnumerable<Folder> folders = await folderService.GetAllAsync();
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
            Collections = folder.Collections?.Select(c => new CollectionSummaryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.GetType().Name,
                CoverUrl = c.Cover?.Url ?? string.Empty
            }).ToList() ?? new List<CollectionSummaryDTO>()
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<FolderDTO>>>(dtos, ["Successfully retrieved all folders"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPost]
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

    // TODO: write XML comments and returnType attributes
    [HttpPut("{folderId:int}/update-name")]
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
            Collections = folder.Collections?.Select(c => new CollectionSummaryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.GetType().Name,
                CoverUrl = c.Cover?.Url ?? string.Empty
            }).ToList() ?? new List<CollectionSummaryDTO>()
        };
        throw ResponseFactory.Create<OkResponse<FolderDTO>>(dto, ["Successfully updated folder name"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpDelete("{folderId:int}")]
    public async Task<IActionResult> DeleteFolder(int folderId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.DeleteFolderAsync(libraryId, folderId);
        throw ResponseFactory.Create<OkResponse>(["Successfully deleted folder"]);
    }

    // TODO: write XML comments and returnType attributes
    // TODO: Implement adding/removing collections to/from folders when collections have been tested
    [HttpPost("{folderId:int}/add-collection/{collectionId:int}")]
    public async Task<IActionResult> AddCollectionToFolder(int folderId, int collectionId)
    {
        throw new NotImplementedException();
    }

    // TODO: write XML comments and returnType attributes
    [HttpDelete("{folderId:int}/remove-collection/{collectionId:int}")]
    public async Task<IActionResult> RemoveCollectionFromFolder(int folderId, int collectionId)
    {
        throw new NotImplementedException();
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("{folderId:int}/move/{newParentFolderId:int}")]
    public async Task<IActionResult> MoveFolder(int folderId, int newParentFolderId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.MoveFolder(libraryId, folderId, newParentFolderId);
        throw ResponseFactory.Create<OkResponse>(["Folder was successfully moved"]);
    }
}