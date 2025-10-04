using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Client
{
    public class PlaybackQualityRepository(SonarContext dbContext) : GenericRepository<PlaybackQuality>(dbContext), IPlaybackQualityRepository
    {
        public async Task<PlaybackQuality> GetDefaultAsync()
        {
            return (await context.PlaybackQualities.FirstOrDefaultAsync(p => p.Id == 2))!;
        }
    }
}
