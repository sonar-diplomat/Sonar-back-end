using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Repository.Client;

public interface IPlaybackQualityRepository : IGenericRepository<PlaybackQuality>
{
    Task<PlaybackQuality> GetDefaultAsync();
}
