using Application.Abstractions.Interfaces.Repository.Report;
using Entities.Models.Report;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Report;

public class ReportableEntityTypeRepository : GenericRepository<ReportableEntityType>, IReportableEntityTypeRepository
{
    public ReportableEntityTypeRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}