using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Entities.Models.Music;

namespace Application.Services.Music;

public class CollectionService<T>(IGenericRepository<T> repository) : GenericService<T>(repository), ICollectionService<T> where T : Collection
{
    private readonly IGenericRepository<T> repository = repository;
    
    // TODO: Move to CollectionController
    // public async Task<IActionResult> ShareLink(int playlistId)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async Task<IActionResult> ShareQr(int playlistId)
    // {
    //     throw new NotImplementedException();
    // }
    
    // TODO: Make this work for any collection type
    public async Task<IEnumerable<Track>> GetAllTracksAsync(int collectionId)
    {
        return (await repository.Include(c => c.Tracks).GetByIdValidatedAsync(collectionId)).Tracks;
    }

    public async Task UpdateVisibilityStatusAsync(int collectionId, int newVisibilityStatusId)
    {
        T collection = await repository.Include(a => a.VisibilityState).GetByIdValidatedAsync(collectionId);
        collection.VisibilityState.StatusId = newVisibilityStatusId;
        await repository.UpdateAsync(collection);
    }
}