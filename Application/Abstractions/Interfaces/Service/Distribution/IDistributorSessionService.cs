using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IDistributorSessionService
    {
        Task<DistributorSession> GetByIdAsync(int id);
        Task<IEnumerable<DistributorSession>> GetAllAsync();
        Task<DistributorSession> CreateAsync(DistributorSession session);
        Task<DistributorSession> UpdateAsync(DistributorSession session);
        Task<bool> DeleteAsync(int id);
    }
}

