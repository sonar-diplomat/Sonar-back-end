using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Report;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IReportableEntityTypeService
    {
        Task<ReportableEntityType> GetByIdAsync(int id);
        Task<IEnumerable<ReportableEntityType>> GetAllAsync();
        Task<ReportableEntityType> CreateAsync(ReportableEntityType entityType);
        Task<ReportableEntityType> UpdateAsync(ReportableEntityType entityType);
        Task<bool> DeleteAsync(int id);
    }
}

