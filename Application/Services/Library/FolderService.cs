using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Extensions;
using Application.Response;
using Entities.Models.Library;
using Microsoft.EntityFrameworkCore;
using LibraryModel = Entities.Models.Library.Library;

namespace Application.Services.Library;

public class FolderService(
    IFolderRepository repository,
    ILibraryRepository libraryRepository
) : GenericService<Folder>(repository), IFolderService
{
    public async Task<Folder> CreateFolderAsync(int libraryId, CreateFolderDTO dto)
    {
        int? parentFolderId = dto.ParentFolderId;
        
        // Если parentFolderId == null || 0, используем root папку библиотеки
        if (parentFolderId == null || parentFolderId == 0)
        {
            // Получаем библиотеку и её RootFolderId напрямую через репозиторий
            LibraryModel? library = await libraryRepository.GetByIdAsync(libraryId);
            if (library == null)
                throw ResponseFactory.Create<NotFoundResponse>(["Library not found"]);
            if (library.RootFolderId == null)
                throw ResponseFactory.Create<NotFoundResponse>(["Root folder not found for this library"]);
            
            // Получаем root папку через свой метод
            Folder rootFolder = await GetFolderByIdIncludeCollectionsValidatedAsync(library.RootFolderId.Value);
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
            .GetByIdValidatedAsync(folderId);
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


    private async Task<Folder> CheckFolderBelongsToLibrary(int libraryId, int folderId)
    {
        Folder folder = await repository.SnInclude(f => f.Library).GetByIdValidatedAsync(folderId);
        return folder.Library.Id != libraryId
            ? throw ResponseFactory.Create<BadRequestResponse>(["Folder does not belong to the specified library"])
            : folder;
    }
}