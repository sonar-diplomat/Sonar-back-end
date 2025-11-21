using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Application.Response;
using Entities.Models.Library;
using Entities.Models.Music;
using LibraryModel = Entities.Models.Library.Library;

namespace Application.Services.Library;

public class LibraryService(
    ILibraryRepository repository,
    IFolderService folderService
) : GenericService<LibraryModel>(repository), ILibraryService
{
    private const string DefaultRootFolderName = "Root";

    public async Task<LibraryModel> CreateDefaultAsync()
    {
        LibraryModel library = new();
        await repository.AddAsync(library);
        Folder rootFolder = new()
        {
            Name = DefaultRootFolderName,
            IsProtected = true,
            LibraryId = library.Id
        };
        rootFolder = await folderService.CreateAsync(rootFolder);
        library.RootFolderId = rootFolder.Id;
        await repository.UpdateAsync(library);
        return await repository.Include(l => l.RootFolder).ThenInclude(c => c!.Collections)
            .GetByIdValidatedAsync(library.Id);
    }

    public async Task<Folder> GetRootFolderByLibraryIdValidatedAsync(int libraryId)
    {
        Folder rootFolder =
            await folderService.GetFolderByIdIncludeCollectionsValidatedAsync(
                (int)(await GetByIdValidatedAsync(libraryId)).RootFolderId!);
        return rootFolder ?? throw ResponseFactory.Create<NotFoundResponse>(["Root folder not found"]);
    }

    public async Task<Playlist> GetFavoritesPlaylistByLibraryIdValidatedAsync(int libraryId)
    {
        LibraryModel library = await repository
            .Include(l => l.RootFolder)
            .ThenInclude(rf => rf!.Collections)
            .GetByIdValidatedAsync(libraryId);
        return library.RootFolder!.Collections.FirstOrDefault(c => c.Name == "Favorites") as Playlist
               ?? throw ResponseFactory.Create<NotFoundResponse>(["Favorites playlist not found"]);
    }
}