namespace Application.Abstractions.Interfaces.Services;

public interface IFolderCollectionService
{
    Task AddCollectionToFolderAsync(int libraryId, int folderId, int collectionId);
    Task RemoveCollectionFromFolderAsync(int libraryId, int folderId, int collectionId);
    Task MoveCollectionToFolderAsync(int libraryId, int collectionId, int targetFolderId);
}

