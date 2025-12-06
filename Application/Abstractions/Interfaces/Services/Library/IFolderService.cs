using Application.DTOs;
using Entities.Models.Library;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface IFolderService : IGenericService<Folder>
{
    Task<Folder> CreateFolderAsync(int libraryId, CreateFolderDTO dto);
    Task<Folder> UpdateNameAsync(int libraryId, int folderId, string newName);
    Task DeleteFolderAsync(int libraryId, int folderId);
    Task MoveFolder(int libraryId, int folderId, int newParentFolderId);
    Task AddCollectionToFolderAsync(int libraryId, int folderId, int collectionId);
    Task RemoveCollectionFromFolderAsync(int libraryId, int folderId, int collectionId);
    Task MoveCollectionToFolderAsync(int libraryId, int collectionId, int targetFolderId);
    Task<Folder> GetFolderByIdIncludeCollectionsValidatedAsync(int folderId);
    Task<Folder> GetFolderByIdIncludeCollectionsValidatedAsync(int folderId, int libraryId);
    Task<IEnumerable<Folder>> GetAllFoldersWithCollectionsAsync();
    Task<IEnumerable<Folder>> GetAllFoldersWithCollectionsByLibraryIdAsync(int libraryId);
}