using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IPlaylistService
    {
        Task<Playlist> GetByIdAsync(int id);
        Task<IEnumerable<Playlist>> GetAllAsync();
        Task<Playlist> CreateAsync(Playlist playlist);
        Task<Playlist> UpdateAsync(Playlist playlist);
        Task<bool> DeleteAsync(int id);
    }
}

