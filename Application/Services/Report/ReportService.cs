using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;

namespace Application.Services.Report
{
    public class ReportService(IReportRepository repository) : IReportService
    {

        public Task<Entities.Models.Report.Report> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Entities.Models.Report.Report>> GetAllAsync() => throw new NotImplementedException();
        public Task<Entities.Models.Report.Report> CreateAsync(Entities.Models.Report.Report entity) => throw new NotImplementedException();
        public Task<Entities.Models.Report.Report> UpdateAsync(Entities.Models.Report.Report entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

