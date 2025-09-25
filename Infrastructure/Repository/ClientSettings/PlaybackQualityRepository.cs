using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class PlaybackQualityRepository : GenericRepository<PlaybackQuality>, IPlaybackQualityRepository
    {
        public PlaybackQualityRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
