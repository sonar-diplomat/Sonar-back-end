using Application.Abstractions.Interfaces.Repository.Music;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Music
{
    public class AlbumRepository : GenericRepository<Entities.Models.Album>, IAlbumRepository
    {
        public AlbumRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
