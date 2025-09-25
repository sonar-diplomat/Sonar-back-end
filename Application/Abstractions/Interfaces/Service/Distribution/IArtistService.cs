using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IArtistService
    {
        Task<Artist> GetByIdAsync(int id);
        Task<IEnumerable<Artist>> GetAllAsync();
        Task<Artist> CreateAsync(Artist artist);
        Task<Artist> UpdateAsync(Artist artist);
        Task<bool> DeleteAsync(int id);
    }
}

