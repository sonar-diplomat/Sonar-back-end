using Application.Abstractions.Interfaces.Repository.Report;
using Entities.Models.Report;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Report
{
    public class ReportableEntityTypeRepository : GenericRepository<ReportableEntityType>, IReportableEntityTypeRepository
    {
        public ReportableEntityTypeRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
