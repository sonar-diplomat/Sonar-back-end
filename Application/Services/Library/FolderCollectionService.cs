using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Models.Library;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Library;

public class FolderCollectionService(
    IFolderRepository folderRepository,
    IFolderService folderService,
    ICollectionService<Album> albumCollectionService,
    ICollectionService<Playlist> playlistCollectionService,
    ICollectionService<Blend> blendCollectionService
) : IFolderCollectionService
{
    public async Task AddCollectionToFolderAsync(int libraryId, int folderId, int collectionId)
    {
        Folder folder = await folderService.GetFolderByIdIncludeCollectionsValidatedAsync(folderId, libraryId);
        
        // Пытаемся найти коллекцию во всех типах
        Collection? collection = await GetCollectionByIdAsync(collectionId);
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found"]);
        
        // Проверяем, что коллекция еще не в папке
        if (folder.Collections != null && folder.Collections.Any(c => c.Id == collectionId))
            throw ResponseFactory.Create<ConflictResponse>(["Collection is already in this folder"]);
        
        folder.Collections ??= new List<Collection>();
        folder.Collections.Add(collection);
        await folderRepository.UpdateAsync(folder);
    }

    public async Task RemoveCollectionFromFolderAsync(int libraryId, int folderId, int collectionId)
    {
        Folder folder = await folderService.GetFolderByIdIncludeCollectionsValidatedAsync(folderId, libraryId);
        
        // Нельзя удалять коллекции из защищенных папок
        if (folder.IsProtected)
            throw ResponseFactory.Create<ForbiddenResponse>(["Cannot remove collections from protected system folders"]);
        
        Collection? collection = folder.Collections?.FirstOrDefault(c => c.Id == collectionId);
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found in this folder"]);
        
        folder.Collections!.Remove(collection);
        await folderRepository.UpdateAsync(folder);
    }

    public async Task MoveCollectionToFolderAsync(int libraryId, int collectionId, int targetFolderId)
    {
        Folder targetFolder = await folderService.GetFolderByIdIncludeCollectionsValidatedAsync(targetFolderId, libraryId);
        
        Collection? collection = await GetCollectionByIdAsync(collectionId);
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found"]);
        
        // Находим все папки библиотеки, содержащие эту коллекцию
        IQueryable<Folder> allFolders = await folderRepository.GetAllAsync();
        List<Folder> foldersWithCollection = await allFolders
            .Where(f => f.LibraryId == libraryId && f.Collections != null && f.Collections.Any(c => c.Id == collectionId))
            .Include(f => f.Collections)
            .ToListAsync();
        
        // Удаляем коллекцию из всех текущих папок
        foreach (Folder folder in foldersWithCollection)
        {
            if (folder.IsProtected)
                continue; // Не удаляем из защищенных папок
            
            Collection? col = folder.Collections?.FirstOrDefault(c => c.Id == collectionId);
            if (col != null)
                folder.Collections!.Remove(col);
        }
        
        // Добавляем коллекцию в целевую папку
        targetFolder.Collections ??= new List<Collection>();
        if (!targetFolder.Collections.Any(c => c.Id == collectionId))
            targetFolder.Collections.Add(collection);
        
        // Сохраняем изменения
        foreach (Folder folder in foldersWithCollection)
            await folderRepository.UpdateAsync(folder);
        
        await folderRepository.UpdateAsync(targetFolder);
    }

    /// <summary>
    /// Получает коллекцию по ID, проверяя все типы коллекций
    /// </summary>
    private async Task<Collection?> GetCollectionByIdAsync(int collectionId)
    {
        Collection? collection = await playlistCollectionService.GetByIdAsync(collectionId) as Collection;
        if (collection != null)
            return collection;
        
        collection = await albumCollectionService.GetByIdAsync(collectionId) as Collection;
        if (collection != null)
            return collection;
        
        collection = await blendCollectionService.GetByIdAsync(collectionId) as Collection;
        return collection;
    }
}

