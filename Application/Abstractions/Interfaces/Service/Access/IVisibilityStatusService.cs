using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IVisibilityStatusService
    {
        Task<VisibilityStatus> GetByIdAsync(int id);
        Task<IEnumerable<VisibilityStatus>> GetAllAsync();
        Task<VisibilityStatus> CreateAsync(VisibilityStatus status);
        Task<VisibilityStatus> UpdateAsync(VisibilityStatus status);
        Task<bool> DeleteAsync(int id);
    }
}

