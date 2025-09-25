using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models.Music;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Music
{
    public class PlaylistRepository : GenericRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
