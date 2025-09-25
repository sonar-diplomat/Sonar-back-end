using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IThemeService
    {
        Task<Theme> GetByIdAsync(int id);
        Task<IEnumerable<Theme>> GetAllAsync();
        Task<Theme> CreateAsync(Theme theme);
        Task<Theme> UpdateAsync(Theme theme);
        Task<bool> DeleteAsync(int id);
    }
}

