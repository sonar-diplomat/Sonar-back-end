using Application.Abstractions.Interfaces.Repository.Report;
using Entities.Models.Report;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Report;

public class ReportReasonTypeRepository : GenericRepository<ReportReasonType>, IReportReasonTypeRepository
{
    public ReportReasonTypeRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
