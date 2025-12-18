using Application.DTOs;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface IArtistService : IGenericService<Artist>
{
    public Task<Artist?> GetByNameAsync(string name);
    Task<IEnumerable<Artist>> SearchArtistsAsync(string searchTerm);
    Task UpdateNameAsync(int artistId, string newArtistName);
    Task DeleteArtistAsync(int artistId);
    Task CreatePostAsync(int artistId, PostDTO dto);
    Task UpdatePostAsync(int postId, int artistId, PostDTO dto);

    Task<Artist> RegisterArtistAsync(int userId, string artistName);
    Task<Artist?> GetByIdWithUserAsync(int artistId);
}