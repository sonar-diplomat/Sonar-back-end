using Application.Abstractions.Interfaces.Repository.Access;
using Application.Response;
using Entities.Models.Access;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Access;

public class VisibilityStatusRepository(SonarContext dbContext)
    : GenericRepository<VisibilityStatus>(dbContext), IVisibilityStatusRepository
{
    public async Task<VisibilityStatus> GetDefaultAsync()
    {
        return await dbContext.VisibilityStatuses.FirstOrDefaultAsync(vs => vs.Name == "Hidden")
               ?? throw ResponseFactory.Create<ExpectationFailedResponse>(["Default VisibilityStatus not found"]);
    }
}
