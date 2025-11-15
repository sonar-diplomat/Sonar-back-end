using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Entities.Models.Library;
using Entities.Models.Music;

namespace Application.Services.Music;

public class CollectionService<T>(
    IGenericRepository<T> repository,
    ILibraryService libraryService,
    IFolderService folderService)
    : GenericService<T>(repository), ICollectionService<T> where T : Collection
{
    private readonly IGenericRepository<T> repository = repository;

    public async Task<IEnumerable<Track>> GetAllTracksAsync(int collectionId)
    {
        return (await repository.SnInclude(c => c.Tracks).GetByIdValidatedAsync(collectionId)).Tracks;
    }

    public async Task UpdateVisibilityStatusAsync(int collectionId, int newVisibilityStatusId)
    {
        T collection = await repository.SnInclude(a => a.VisibilityState).GetByIdValidatedAsync(collectionId);
        collection.VisibilityState.StatusId = newVisibilityStatusId;
        await repository.UpdateAsync(collection);
    }

    public async Task ToggleFavoriteAsync(int libraryId, int collectionId)
    {
        Folder rootFolder = await libraryService.GetRootFolderByLibraryIdValidatedAsync(libraryId);
        T collection = await GetByIdValidatedAsync(collectionId);
        rootFolder.Collections.Add(collection);
        await folderService.UpdateAsync(rootFolder);
    }
}