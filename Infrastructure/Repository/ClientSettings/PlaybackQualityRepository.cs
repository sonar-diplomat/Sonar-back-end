using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.ClientSettings
{
    public class PlaybackQualityRepository(SonarContext dbContext) 
        : GenericRepository<PlaybackQuality>(dbContext), IPlaybackQualityRepository
    {
        public async Task<PlaybackQuality> GetDefaultAsync()
        {
            return (await context.PlaybackQualities.FirstOrDefaultAsync(p => p.Id == 2))!;
        }
    }
}