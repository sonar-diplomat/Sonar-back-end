using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Extensions;
using Application.Response;
using Entities.Enums;
using Entities.Models.Library;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Library;

public class FolderService(
    IFolderRepository repository,
    ICollectionService<Album> albumCollectionService,
    ICollectionService<Playlist> playlistCollectionService,
    ICollectionService<Blend> blendCollectionService
) : GenericService<Folder>(repository), IFolderService
{
    public async Task<Folder> CreateFolderAsync(int libraryId, CreateFolderDTO dto)
    {
        int? parentFolderId = dto.ParentFolderId;

        // Если parentFolderId == null || 0, используем root папку библиотеки
        if (parentFolderId is null or 0)
        {
            // Получаем RootFolderId напрямую через репозиторий
            int? rootFolderId = await repository.GetRootFolderIdByLibraryIdAsync(libraryId);

            if (rootFolderId == null)
                throw ResponseFactory.Create<NotFoundResponse>(["Root folder not found for this library"]);

            // Получаем root папку через свой метод
            Folder rootFolder = await GetFolderByIdIncludeCollectionsValidatedAsync(rootFolderId.Value);
            parentFolderId = rootFolder.Id;
        }
        else
        {
            // Проверяем, что указанная папка принадлежит библиотеке
            await CheckFolderBelongsToLibrary(libraryId, parentFolderId.Value);
        }

        Folder newFolder = new()
        {
            Name = dto.Name,
            LibraryId = libraryId,
            ParentFolderId = parentFolderId
        };
        return await repository.AddAsync(newFolder);
    }

    public async Task<Folder> UpdateNameAsync(int libraryId, int folderId, string newName)
    {
        Folder folder = await CheckFolderBelongsToLibrary(libraryId, folderId);
        folder.Name = newName;
        return await repository.UpdateAsync(folder);
    }

    public async Task DeleteFolderAsync(int libraryId, int folderId)
    {
        Folder folder = await CheckFolderBelongsToLibrary(libraryId, folderId);
        if (folder.IsProtected)
            throw ResponseFactory.Create<ForbiddenResponse>(["This folder is protected and cannot be deleted"]);
        await repository.RemoveAsync(folder);
    }

    public async Task MoveFolder(int libraryId, int folderId, int newParentFolderId)
    {
        Folder folder = await CheckFolderBelongsToLibrary(libraryId, folderId);
        
        // Нельзя переместить папку в саму себя
        if (folderId == newParentFolderId)
            throw ResponseFactory.Create<BadRequestResponse>(["Cannot move folder into itself"]);
        
        // Нельзя переместить защищенную папку
        if (folder.IsProtected)
            throw ResponseFactory.Create<ForbiddenResponse>(["Cannot move protected system folders"]);
        
        Folder parentFolder = await CheckFolderBelongsToLibrary(libraryId, newParentFolderId);
        
        // Проверка на циклические ссылки: нельзя переместить папку в её потомка
        // Проверяем, является ли newParentFolderId потомком folderId
        if (await IsDescendantOfAsync(folderId, newParentFolderId))
            throw ResponseFactory.Create<BadRequestResponse>(["Cannot move folder into its descendant"]);
        
        folder.ParentFolderId = parentFolder.Id;
        await repository.UpdateAsync(folder);
    }

    public async Task AddCollectionToFolderAsync(int libraryId, int folderId, int collectionId)
    {
        Folder folder = await CheckFolderBelongsToLibrary(libraryId, folderId);
        
        // Пытаемся найти коллекцию во всех типах
        Collection? collection = await GetCollectionByIdAsync(collectionId);
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found"]);
        
        // Проверяем, что коллекция еще не в папке
        if (folder.Collections != null && folder.Collections.Any(c => c.Id == collectionId))
            throw ResponseFactory.Create<ConflictResponse>(["Collection is already in this folder"]);
        
        folder.Collections ??= new List<Collection>();
        folder.Collections.Add(collection);
        await repository.UpdateAsync(folder);
    }

    public async Task RemoveCollectionFromFolderAsync(int libraryId, int folderId, int collectionId)
    {
        Folder folder = await CheckFolderBelongsToLibrary(libraryId, folderId);
        
        // Нельзя удалять коллекции из защищенных папок
        if (folder.IsProtected)
            throw ResponseFactory.Create<ForbiddenResponse>(["Cannot remove collections from protected system folders"]);
        
        Collection? collection = folder.Collections?.FirstOrDefault(c => c.Id == collectionId);
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found in this folder"]);
        
        folder.Collections!.Remove(collection);
        await repository.UpdateAsync(folder);
    }

    public async Task MoveCollectionToFolderAsync(int libraryId, int collectionId, int targetFolderId)
    {
        Folder targetFolder = await CheckFolderBelongsToLibrary(libraryId, targetFolderId);
        
        Collection? collection = await GetCollectionByIdAsync(collectionId);
        if (collection == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Collection not found"]);
        
        // Находим все папки библиотеки, содержащие эту коллекцию
        IQueryable<Folder> allFolders = await repository.GetAllAsync();
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
            await repository.UpdateAsync(folder);
        
        await repository.UpdateAsync(targetFolder);
    }

    public async Task<Folder> GetFolderByIdIncludeCollectionsValidatedAsync(int folderId)
    {
        return await repository
            .SnInclude(f => f.Collections)
            .ThenInclude(c => c.VisibilityState)
            .ThenInclude(vs => vs.Status)
            .SnInclude(f => f.SubFolders)
            .SnInclude(f => f.ParentFolder)
            .GetByIdValidatedAsync(folderId);
    }

    public async Task<Folder> GetFolderByIdIncludeCollectionsValidatedAsync(int folderId, int libraryId)
    {
        Folder folder = await repository
            .SnInclude(f => f.Library)
            .SnInclude(f => f.Collections)
            .ThenInclude(c => c.VisibilityState)
            .ThenInclude(vs => vs.Status)
            .SnInclude(f => f.Collections)
            .ThenInclude(c => (c as Playlist).Creator)
            .SnInclude(f => f.Collections)
            .ThenInclude(c => (c as Album).AlbumArtists)
            .ThenInclude(aa => aa.Artist)
            .SnInclude(f => f.SubFolders)
            .SnInclude(f => f.ParentFolder)
            .GetByIdValidatedAsync(folderId);

        if (folder.Library.Id != libraryId)
            throw ResponseFactory.Create<NotFoundResponse>(["Folder not found"]);

        return folder;
    }

    public async Task<IEnumerable<Folder>> GetAllFoldersWithCollectionsAsync()
    {
        IQueryable<Folder> query = await repository.GetAllAsync();
        return query
            .Include(f => f.Collections)
            .ThenInclude(c => c.VisibilityState)
            .ThenInclude(vs => vs.Status)
            .Include(f => f.SubFolders)
            .Include(f => f.ParentFolder)
            .ToList();
    }

    public async Task<IEnumerable<Folder>> GetAllFoldersWithCollectionsByLibraryIdAsync(int libraryId)
    {
        IQueryable<Folder> query = await repository.GetAllAsync();
        return query
            .Where(f => f.LibraryId == libraryId)
            .Include(f => f.Collections)
            .ThenInclude(c => c.VisibilityState)
            .ThenInclude(vs => vs.Status)
            .Include(f => f.Collections)
            .ThenInclude(c => (c as Playlist).Creator)
            .Include(f => f.Collections)
            .ThenInclude(c => (c as Album).AlbumArtists)
            .ThenInclude(aa => aa.Artist)
            .Include(f => f.SubFolders)
            .Include(f => f.ParentFolder)
            .ToList();
    }


    private async Task<Folder> CheckFolderBelongsToLibrary(int libraryId, int folderId)
    {
        Folder folder = await repository.SnInclude(f => f.Library).GetByIdValidatedAsync(folderId);
        return folder.Library.Id != libraryId
            ? throw ResponseFactory.Create<BadRequestResponse>(["Folder does not belong to the specified library"])
            : folder;
    }

    /// <summary>
    /// Проверяет, является ли descendantId потомком ancestorId (прямым или косвенным)
    /// </summary>
    private async Task<bool> IsDescendantOfAsync(int ancestorId, int descendantId)
    {
        if (ancestorId == descendantId)
            return true;
        
        IQueryable<Folder> allFolders = await repository.GetAllAsync();
        List<Folder> folders = await allFolders
            .Include(f => f.SubFolders)
            .ToListAsync();
        
        Dictionary<int, Folder> folderDict = folders.ToDictionary(f => f.Id);
        
        // Рекурсивно проверяем всех потомков ancestorId
        HashSet<int> visited = new();
        Queue<int> queue = new();
        queue.Enqueue(ancestorId);
        
        while (queue.Count > 0)
        {
            int currentId = queue.Dequeue();
            if (visited.Contains(currentId))
                continue;
            
            visited.Add(currentId);
            
            if (currentId == descendantId)
                return true;
            
            if (folderDict.TryGetValue(currentId, out Folder? currentFolder) && currentFolder.SubFolders != null)
            {
                foreach (Folder subFolder in currentFolder.SubFolders)
                {
                    if (!visited.Contains(subFolder.Id))
                        queue.Enqueue(subFolder.Id);
                }
            }
        }
        
        return false;
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