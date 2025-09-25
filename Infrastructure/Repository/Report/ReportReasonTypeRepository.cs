using Application.Abstractions.Interfaces.Repository.Report;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Report
{
    public class ReportReasonTypeRepository : GenericRepository<Entities.Models.ReportReasonType>, IReportReasonTypeRepository
    {
        public ReportReasonTypeRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
