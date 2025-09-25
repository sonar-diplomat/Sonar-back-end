using Application.Abstractions.Interfaces.Repository.Music;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Music
{
    public class PlaylistRepository : GenericRepository<Entities.Models.Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
