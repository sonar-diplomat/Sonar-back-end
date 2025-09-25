using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ISuspensionService
    {
        Task<Suspension> GetByIdAsync(int id);
        Task<IEnumerable<Suspension>> GetAllAsync();
        Task<Suspension> CreateAsync(Suspension suspension);
        Task<Suspension> UpdateAsync(Suspension suspension);
        Task<bool> DeleteAsync(int id);
    }
}

