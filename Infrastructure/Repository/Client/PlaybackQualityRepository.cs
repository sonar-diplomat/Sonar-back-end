using Application.Abstractions.Interfaces.Repository.Client;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class PlaybackQualityRepository : GenericRepository<Entities.Models.PlaybackQuality>, IPlaybackQualityRepository
    {
        public PlaybackQualityRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
