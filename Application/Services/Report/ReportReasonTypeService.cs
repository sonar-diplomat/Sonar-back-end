using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Report;

namespace Application.Services.Report
{
    public class ReportReasonTypeService(IReportReasonTypeRepository repository) : IReportReasonTypeService
    {

        public Task<ReportReasonType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<ReportReasonType>> GetAllAsync() => throw new NotImplementedException();
        public Task<ReportReasonType> CreateAsync(ReportReasonType entity) => throw new NotImplementedException();
        public Task<ReportReasonType> UpdateAsync(ReportReasonType entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

