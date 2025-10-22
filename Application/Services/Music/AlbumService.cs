using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Music;

public class AlbumService(IAlbumRepository repository) : GenericService<Album>(repository), IAlbumService
{
    public async Task<Album> UpdateNameAsync(int albumId, string newName)
    {
        Album album = await GetByIdValidatedAsync(albumId);
        album.Name = newName;
        return await repository.UpdateAsync(album);
    }
}