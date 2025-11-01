using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.Chat;
using Entities.Models.Music;

namespace Application.Services.Music;

public class ArtistService(IArtistRepository repository, IPostService postService)
    : GenericService<Artist>(repository), IArtistService
{
    public async Task<Artist?> GetByNameAsync(string name)
    {
        return await repository.GetByNameAsync(name);
    }

    public async Task UpdateNameAsync(int artistId, string newArtistName)
    {
        Artist artist = await GetByIdValidatedAsync(artistId);
        artist.ArtistName = newArtistName;
        await repository.UpdateAsync(artist);
    }

    public async Task DeleteArtistAsync(int artistId)
    {
        Artist artist = await GetByIdValidatedAsync(artistId);
        foreach (Post post in artist.Posts) await postService.DeleteAsync(post);
        await repository.RemoveAsync(artist);
    }

    public async Task CreatePostAsync(int artistId, PostDTO dto)
    {
        await GetByIdValidatedAsync(artistId);
        await postService.CreateAsync(artistId, dto);
    }

    public async Task UpdatePostAsync(int postId, int artistId, PostDTO dto)
    {
        await GetByIdValidatedAsync(artistId);
        await postService.UpdateAsync(postId, dto);
    }

    public async Task<Artist> RegisterArtistAsync(int userId, string artistName)
    {
        Artist artist = new()
        {
            Id = userId,
            UserId = userId,
            ArtistName = artistName
        };
        return await repository.AddAsync(artist);
    }
}