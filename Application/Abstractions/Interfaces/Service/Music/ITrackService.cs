using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ITrackService
    {
        Task<Track> GetByIdAsync(int id);
        Task<IEnumerable<Track>> GetAllAsync();
        Task<Track> CreateAsync(Track track);
        Task<Track> UpdateAsync(Track track);
        Task<bool> DeleteAsync(int id);
    }
}

