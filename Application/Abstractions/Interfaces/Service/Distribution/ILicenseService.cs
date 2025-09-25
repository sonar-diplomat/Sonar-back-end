using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ILicenseService
    {
        Task<License> GetByIdAsync(int id);
        Task<IEnumerable<License>> GetAllAsync();
        Task<License> CreateAsync(License license);
        Task<License> UpdateAsync(License license);
        Task<bool> DeleteAsync(int id);
    }
}

