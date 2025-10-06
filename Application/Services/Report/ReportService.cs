using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;
using ReportModel = Entities.Models.Report.Report;

namespace Application.Services.Report;

public class ReportService(IReportRepository repository) : GenericService<ReportModel>(repository), IReportService
{
}