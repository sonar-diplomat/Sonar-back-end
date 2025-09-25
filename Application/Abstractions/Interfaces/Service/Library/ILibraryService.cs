using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Library;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ILibraryService
    {
        Task<Library> GetByIdAsync(int id);
        Task<IEnumerable<Library>> GetAllAsync();
        Task<Library> CreateAsync(Library library);
        Task<Library> UpdateAsync(Library library);
        Task<bool> DeleteAsync(int id);
    }
}
