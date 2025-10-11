using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models.Access;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Access;

public class SuspensionRepository : GenericRepository<Suspension>, ISuspensionRepository
{
    public SuspensionRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
