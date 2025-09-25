using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IPlaybackQualityService
    {
        Task<PlaybackQuality> GetByIdAsync(int id);
        Task<IEnumerable<PlaybackQuality>> GetAllAsync();
        Task<PlaybackQuality> CreateAsync(PlaybackQuality playbackQuality);
        Task<PlaybackQuality> UpdateAsync(PlaybackQuality playbackQuality);
        Task<bool> DeleteAsync(int id);
    }
}

