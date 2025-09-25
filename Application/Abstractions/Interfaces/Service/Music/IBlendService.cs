using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IBlendService
    {
        Task<Blend> GetByIdAsync(int id);
        Task<IEnumerable<Blend>> GetAllAsync();
        Task<Blend> CreateAsync(Blend blend);
        Task<Blend> UpdateAsync(Blend blend);
        Task<bool> DeleteAsync(int id);
    }
}

