using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IVisibilityStateService
    {
        Task<VisibilityState> GetByIdAsync(int id);
        Task<IEnumerable<VisibilityState>> GetAllAsync();
        Task<VisibilityState> CreateAsync(VisibilityState state);
        Task<VisibilityState> UpdateAsync(VisibilityState state);
        Task<bool> DeleteAsync(int id);
    }
}

