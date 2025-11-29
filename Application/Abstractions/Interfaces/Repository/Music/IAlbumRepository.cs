using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Repository.Music;

public interface IAlbumRepository : IGenericRepository<Album>
{
    Task<IEnumerable<Album>> GetByDistributorIdAsync(int distributorId);
    Task<List<Track>> GetTracksFromAlbumAsync(int albumId);
}
