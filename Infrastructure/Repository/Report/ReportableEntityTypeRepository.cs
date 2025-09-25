using Application.Abstractions.Interfaces.Repository.Report;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Report
{
    public class ReportableEntityTypeRepository : GenericRepository<Entities.Models.ReportableEntityType>, IReportableEntityTypeRepository
    {
        public ReportableEntityTypeRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
