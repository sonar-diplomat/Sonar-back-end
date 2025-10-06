using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Report;

namespace Application.Services.Report;

public class ReportReasonTypeService(IReportReasonTypeRepository repository)
    : GenericService<ReportReasonType>(repository), IReportReasonTypeService
{
}