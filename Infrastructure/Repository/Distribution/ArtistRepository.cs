using Application.Abstractions.Interfaces.Repository.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class ArtistRepository : GenericRepository<Entities.Models.Artist>, IArtistRepository
    {
        public ArtistRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
