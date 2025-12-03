using Application.DTOs;
using Entities.Models.Library;

namespace Application.Abstractions.Interfaces.Services;

public interface IFolderService : IGenericService<Folder>
{
    Task<Folder> CreateFolderAsync(int libraryId, CreateFolderDTO dto);
    Task<Folder> UpdateNameAsync(int libraryId, int folderId, string newName);
    Task DeleteFolderAsync(int libraryId, int folderId);
    Task MoveFolder(int libraryId, int folderId, int newParentFolderId);
    Task<Folder> GetFolderByIdIncludeCollectionsValidatedAsync(int folderId);
    Task<Folder> GetFolderByIdIncludeCollectionsValidatedAsync(int folderId, int libraryId);
    Task<IEnumerable<Folder>> GetAllFoldersWithCollectionsAsync();
    Task<IEnumerable<Folder>> GetAllFoldersWithCollectionsByLibraryIdAsync(int libraryId);
}