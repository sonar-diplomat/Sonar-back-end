using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Music;

public class AlbumService(IAlbumRepository repository) : IAlbumService
{
    public Task<Album> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Album>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Album> CreateAsync(Album album)
    {
        throw new NotImplementedException();
    }

    public Task<Album> UpdateAsync(Album album)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}