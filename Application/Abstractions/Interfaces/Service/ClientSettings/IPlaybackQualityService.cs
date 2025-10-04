using Application.Abstractions.Interfaces.Service;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IPlaybackQualityService : IGenericService<PlaybackQuality>
    {
        Task<PlaybackQuality> GetDefaultAsync();
    }
}

