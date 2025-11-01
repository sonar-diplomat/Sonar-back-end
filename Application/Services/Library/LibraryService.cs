using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Library;
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
        return await repository.UpdateAsync(library);
    }
}