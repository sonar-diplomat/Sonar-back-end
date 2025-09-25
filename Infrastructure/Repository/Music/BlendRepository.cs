using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models.Music;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Music
{
    public class BlendRepository : GenericRepository<Blend>, IBlendRepository
    {
        public BlendRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
