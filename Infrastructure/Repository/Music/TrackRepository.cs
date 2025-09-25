using Application.Abstractions.Interfaces.Repository.Music;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Music
{
    public class TrackRepository : GenericRepository<Entities.Models.Track>, ITrackRepository
    {
        public TrackRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
