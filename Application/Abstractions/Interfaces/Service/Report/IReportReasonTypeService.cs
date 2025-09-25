using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Report;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IReportReasonTypeService
    {
        Task<ReportReasonType> GetByIdAsync(int id);
        Task<IEnumerable<ReportReasonType>> GetAllAsync();
        Task<ReportReasonType> CreateAsync(ReportReasonType reasonType);
        Task<ReportReasonType> UpdateAsync(ReportReasonType reasonType);
        Task<bool> DeleteAsync(int id);
    }
}

