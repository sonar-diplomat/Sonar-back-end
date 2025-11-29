using Application.DTOs.Report;
using Entities.Models.Report;

namespace Application.Abstractions.Interfaces.Services;

public interface IReportService : IGenericService<Report>
{
    Task<Report> CreateReportAsync(int userId, CreateReportDTO dto);
    Task CloseReportAsync(int reportId);
    Task<IEnumerable<Report>> GetReportsFilteredAsync(ReportFilterDTO dto);
    Task<IEnumerable<Report>> GetReportsByReporterAsync(int reporterId);
    Task<IEnumerable<Report>> GetOpenReportsAsync();
    Task<Report> GetByIdValidatedFullAsync(int id);
}
