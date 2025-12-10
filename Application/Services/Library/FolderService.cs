using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Extensions;
using Application.Response;
using Entities.Models.Library;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Library;

public class FolderService(
    IFolderRepository repository
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
        
        // Собираем все ID дочерних папок для удаления (рекурсивно)
        HashSet<int> subFolderIds = await GetAllSubFolderIdsRecursivelyAsync(folderId);
        
        // Если есть дочерние папки, загружаем их со связями с коллекциями и удаляем
        if (subFolderIds.Any())
        {
            IQueryable<Folder> allFoldersQuery = await repository.GetAllAsync();
            List<Folder> foldersToDelete = await allFoldersQuery
                .Where(f => subFolderIds.Contains(f.Id))
                .Include(f => f.Collections)
                .ToListAsync();
            
            // Очищаем связи с коллекциями для всех дочерних папок
            foreach (Folder folderToDelete in foldersToDelete)
            {
                if (folderToDelete.Collections != null && folderToDelete.Collections.Any())
                {
                    folderToDelete.Collections.Clear();
                }
            }
            
            // Удаляем все дочерние папки
            await repository.RemoveRangeAsync(foldersToDelete);
        }
        
        // Загружаем саму папку со связями с коллекциями
        Folder folderToRemove = await repository
            .SnInclude(f => f.Collections)
            .GetByIdValidatedAsync(folderId);
        
        // Очищаем связи с коллекциями для самой папки
        if (folderToRemove.Collections != null && folderToRemove.Collections.Any())
        {
            folderToRemove.Collections.Clear();
        }
        
        // Удаляем саму папку
        await repository.RemoveAsync(folderToRemove);
    }
    
    /// <summary>
    /// Рекурсивно получает все ID дочерних папок для удаления
    /// </summary>
    private async Task<HashSet<int>> GetAllSubFolderIdsRecursivelyAsync(int folderId)
    {
        HashSet<int> allSubFolderIds = new();
        HashSet<int> processedIds = new();
        Queue<int> queue = new();
        queue.Enqueue(folderId);
        
        while (queue.Count > 0)
        {
            int currentFolderId = queue.Dequeue();
            if (processedIds.Contains(currentFolderId))
                continue;
            
            processedIds.Add(currentFolderId);
            
            // Получаем все дочерние папки текущей папки (только незащищенные)
            IQueryable<Folder> allFoldersQuery = await repository.GetAllAsync();
            List<Folder> subFolders = await allFoldersQuery
                .Where(f => f.ParentFolderId == currentFolderId && !f.IsProtected)
                .ToListAsync();
            
            foreach (Folder subFolder in subFolders)
            {
                if (!processedIds.Contains(subFolder.Id))
                {
                    allSubFolderIds.Add(subFolder.Id);
                    queue.Enqueue(subFolder.Id);
                }
            }
        }
        
        return allSubFolderIds;
    }

    public async Task MoveFolder(int libraryId, int folderId, int newParentFolderId)
    {
        Folder folder = await CheckFolderBelongsToLibrary(libraryId, folderId);
        
        if (folderId == newParentFolderId)
            throw ResponseFactory.Create<BadRequestResponse>(["Cannot move folder into itself"]);
        
        if (folder.IsProtected)
            throw ResponseFactory.Create<ForbiddenResponse>(["Cannot move protected system folders"]);
        
        Folder parentFolder = await CheckFolderBelongsToLibrary(libraryId, newParentFolderId);
        
        if (await IsDescendantOfAsync(folderId, newParentFolderId))
            throw ResponseFactory.Create<BadRequestResponse>(["Cannot move folder into its descendant"]);
        
        folder.ParentFolderId = parentFolder.Id;
        await repository.UpdateAsync(folder);
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
}