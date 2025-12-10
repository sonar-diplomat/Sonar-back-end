using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Entities.Models.Report;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Report;

public class ReportReasonTypeService(IReportReasonTypeRepository repository)
    : GenericService<ReportReasonType>(repository), IReportReasonTypeService
{
    public async Task<IQueryable<ReportReasonType>> GetAllQueryableAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<IEnumerable<ReportReasonType>> GetByEntityTypeIdAsync(int entityTypeId)
    {
        var allReasons = await repository.GetAllAsync();
        return await allReasons
            .SnInclude(rrt => rrt.ApplicableEntityTypes)
            .Where(rrt => rrt.ApplicableEntityTypes.Any(aet => aet.Id == entityTypeId))
            .ToListAsync();
    }
}
