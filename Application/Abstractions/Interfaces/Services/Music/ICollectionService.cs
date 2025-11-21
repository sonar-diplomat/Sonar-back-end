using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface ICollectionService<T> : IGenericService<T> where T : Collection
{
    Task UpdateVisibilityStatusAsync(int collectionId, int newVisibilityStatusId);
    Task<IEnumerable<Track>> GetAllTracksAsync(int collectionId);
    Task ToggleFavoriteAsync(int libraryId, int collectionId);
}