using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Distribution
{
    public class ArtistRepository : GenericRepository<Artist>, IArtistRepository
    {
        public ArtistRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
