using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
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
    // TODO: Make it work only with a user from JWT
    [HttpGet("{folderId:int}")]
    public async Task<IActionResult> GetFolder(int folderId)
    {
        Folder folder = await folderService.GetByIdValidatedAsync(folderId);
        throw ResponseFactory.Create<OkResponse<Folder>>(folder, ["Successfully retrieved folder"]);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFolders()
    {
        IEnumerable<Folder> folders = await folderService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<Folder>>>(folders, ["Successfully retrieved all folders"]);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderDTO request)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        Folder folder = await folderService.CreateFolderAsync(libraryId, request);
        throw ResponseFactory.Create<OkResponse<Folder>>(folder, ["Successfully created folder"]);
    }

    [HttpPut("{folderId:int}/update-name")]
    public async Task<IActionResult> UpdateFolderName(int folderId, [FromBody] string newName)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        Folder folder = await folderService.UpdateNameAsync(libraryId, folderId, newName);
        throw ResponseFactory.Create<OkResponse<Folder>>(folder, ["Successfully updated folder name"]);
    }

    [HttpDelete("{folderId:int}")]
    public async Task<IActionResult> DeleteFolder(int folderId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.DeleteFolderAsync(libraryId, folderId);
        throw ResponseFactory.Create<OkResponse>(["Successfully deleted folder"]);
    }

    // TODO: Implement adding/removing collections to/from folders when collections have been tested
    [HttpPost("{folderId:int}/add-collection/{collectionId:int}")]
    public async Task<IActionResult> AddCollectionToFolder(int folderId, int collectionId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{folderId:int}/remove-collection/{collectionId:int}")]
    public async Task<IActionResult> RemoveCollectionFromFolder(int folderId, int collectionId)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{folderId:int}/move/{newParentFolderId:int}")]
    public async Task<IActionResult> MoveFolder(int folderId, int newParentFolderId)
    {
        int libraryId = (await CheckAccessFeatures([])).LibraryId;
        await folderService.MoveFolder(libraryId, folderId, newParentFolderId);
        throw ResponseFactory.Create<OkResponse>(["Folder was successfully moved"]);
    }
}