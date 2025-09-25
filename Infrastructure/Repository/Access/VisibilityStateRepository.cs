using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models;
using Entities.Models.Access;
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
