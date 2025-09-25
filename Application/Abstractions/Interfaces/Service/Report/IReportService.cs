using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Report;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IReportService
    {
        Task<Report> GetByIdAsync(int id);
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report> CreateAsync(Report report);
        Task<Report> UpdateAsync(Report report);
        Task<bool> DeleteAsync(int id);
    }
}

