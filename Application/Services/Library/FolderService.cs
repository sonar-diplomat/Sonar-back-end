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
        await repository.RemoveAsync(folder);
    }

    public async Task MoveFolder(int libraryId, int folderId, int newParentFolderId)
    {
        Folder folder = await CheckFolderBelongsToLibrary(libraryId, folderId);
        Folder parentFolder = await CheckFolderBelongsToLibrary(libraryId, newParentFolderId);
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
}