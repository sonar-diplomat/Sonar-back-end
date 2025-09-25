using Application.Abstractions.Interfaces.Repository.Music;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Music
{
    public class BlendRepository : GenericRepository<Entities.Models.Blend>, IBlendRepository
    {
        public BlendRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
