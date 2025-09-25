using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Access
{
    public class VisibilityStateRepository : GenericRepository<VisibilityState>, IVisibilityStateRepository
    {
        public VisibilityStateRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
