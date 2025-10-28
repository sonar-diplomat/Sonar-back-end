using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface IArtistService : IGenericService<Artist>
{
    public Task<Artist?> GetByNameAsync(string name);
}