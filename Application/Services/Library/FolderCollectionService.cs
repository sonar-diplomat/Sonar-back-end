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

        Collection? collection = folder.Collections.FirstOrDefault(c => c.Id == collectionId);
        
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found in this folder"]);
        
        if (collection.Name == "Favorites")
            throw ResponseFactory.Create<ForbiddenResponse>(["Cannot remove the Favorites collection"]);

        folder.Collections.Remove(collection);
        await folderRepository.UpdateAsync(folder);
    }

    public async Task MoveCollectionToFolderAsync(int libraryId, int collectionId, int targetFolderId)
    {
        Collection? collection = await GetCollectionByIdAsync(collectionId);
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found"]);
        
        if (collection.Name == "Favorites")
            throw ResponseFactory.Create<ForbiddenResponse>(["Cannot move the Favorites collection"]);

        // Находим все папки библиотеки с коллекциями (включая целевую)
        IQueryable<Folder> allFolders = await folderRepository.GetAllAsync();
        List<Folder> allLibraryFolders = await allFolders
            .Where(f => f.LibraryId == libraryId)
            .Include(f => f.Collections)
            .ToListAsync();

        // Проверяем, что целевая папка существует и принадлежит библиотеке
        Folder? targetFolder = allLibraryFolders.FirstOrDefault(f => f.Id == targetFolderId);
        if (targetFolder == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Target folder not found"]);

        // Фильтруем папки, содержащие эту коллекцию (в памяти)
        List<Folder> foldersWithCollection = allLibraryFolders
            .Where(f => f.Collections.Any(c => c.Id == collectionId))
            .ToList();
        
        foreach (Folder folder in foldersWithCollection)
        {
            // Пропускаем целевую папку (если коллекция уже там, она останется)
            if (folder.Id == targetFolderId)
                continue;
            
            Collection? col = folder.Collections?.FirstOrDefault(c => c.Id == collectionId);
            if (col != null)
            {
                folder.Collections!.Remove(col);
                await folderRepository.UpdateAsync(folder);
            }
        }

        // Добавляем коллекцию в целевую папку, если её там еще нет
        if (targetFolder.Collections.All(c => c.Id != collectionId))
        {
            targetFolder.Collections.Add(collection);
            await folderRepository.UpdateAsync(targetFolder);
        }
    }

    /// <summary>
    /// Получает коллекцию по ID, проверяя все типы коллекций
    /// </summary>
    private async Task<Collection?> GetCollectionByIdAsync(int collectionId)
    {
        Collection? collection = await playlistCollectionService.GetByIdAsync(collectionId);
        if (collection != null)
            return collection;

        collection = await albumCollectionService.GetByIdAsync(collectionId);
        if (collection != null)
            return collection;

        collection = await blendCollectionService.GetByIdAsync(collectionId);
        return collection;
    }
}


