using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IAlbumService
    {
        Task<Album> GetByIdAsync(int id);
        Task<IEnumerable<Album>> GetAllAsync();
        Task<Album> CreateAsync(Album album);
        Task<Album> UpdateAsync(Album album);
        Task<bool> DeleteAsync(int id);
    }
}


