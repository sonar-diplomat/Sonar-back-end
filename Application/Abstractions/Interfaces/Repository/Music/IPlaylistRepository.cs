using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Repository.Music;

public interface IPlaylistRepository : IGenericRepository<Playlist>
{
    Task<List<Track>> GetTracksFromPlaylistAfterAsync(int playlistId, int? afterId, int limit);
}
