using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings;

public class PlaybackQualityService(IPlaybackQualityRepository repository)
    : GenericService<PlaybackQuality>(repository), IPlaybackQualityService
{
    public async Task<PlaybackQuality> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }
}