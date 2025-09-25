using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models.Music;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Music
{
    public class TrackRepository : GenericRepository<Track>, ITrackRepository
    {
        public TrackRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
