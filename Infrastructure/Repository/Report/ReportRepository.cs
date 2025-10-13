using Application.Abstractions.Interfaces.Repository.Report;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Report;

public class ReportRepository : GenericRepository<Entities.Models.Report.Report>, IReportRepository
{
    public ReportRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
