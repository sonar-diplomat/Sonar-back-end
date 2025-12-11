using Entities.Models.Report;

namespace Application.Abstractions.Interfaces.Services;

public interface IReportReasonTypeService : IGenericService<ReportReasonType>
{
    Task<IQueryable<ReportReasonType>> GetAllQueryableAsync();
    Task<IEnumerable<ReportReasonType>> GetByEntityTypeIdAsync(int entityTypeId);
}
