using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models.Access;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Access;

public class VisibilityStatusRepository : GenericRepository<VisibilityStatus>, IVisibilityStatusRepository
{
    public VisibilityStatusRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}