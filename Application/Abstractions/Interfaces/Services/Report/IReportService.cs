using Application.DTOs;
using Entities.Models.Report;

namespace Application.Abstractions.Interfaces.Services;

public interface IReportService : IGenericService<Report>
{
    Task<Report> CreateReportAsync(CreateReportDTO dto);
    Task CloseReportAsync(int reportId);
    Task<IEnumerable<Report>> GetReportsByEntityAsync(int entityId, int entityTypeId);
    Task<IEnumerable<Report>> GetReportsByReporterAsync(int reporterId);
    Task<IEnumerable<Report>> GetOpenReportsAsync();
}
