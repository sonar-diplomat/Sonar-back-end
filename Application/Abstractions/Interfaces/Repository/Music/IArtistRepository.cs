using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Repository.Music;

public interface IArtistRepository : IGenericRepository<Artist>
{
    Task<Artist?> GetByNameAsync(string name);
    Task<IEnumerable<Artist>> SearchByNameAsync(string searchTerm);
}