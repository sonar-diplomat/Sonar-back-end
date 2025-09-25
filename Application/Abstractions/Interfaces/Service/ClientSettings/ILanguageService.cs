using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ILanguageService
    {
        Task<Language> GetByIdAsync(int id);
        Task<IEnumerable<Language>> GetAllAsync();
        Task<Language> CreateAsync(Language language);
        Task<Language> UpdateAsync(Language language);
        Task<bool> DeleteAsync(int id);
    }
}
