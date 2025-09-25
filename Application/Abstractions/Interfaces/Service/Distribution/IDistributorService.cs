using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IDistributorService
    {
        Task<Distributor> GetByIdAsync(int id);
        Task<IEnumerable<Distributor>> GetAllAsync();
        Task<Distributor> CreateAsync(Distributor distributor);
        Task<Distributor> UpdateAsync(Distributor distributor);
        Task<bool> DeleteAsync(int id);
    }
}
