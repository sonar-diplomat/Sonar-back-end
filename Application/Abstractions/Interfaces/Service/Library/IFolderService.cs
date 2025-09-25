using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Library;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IFolderService
    {
        Task<Folder> GetByIdAsync(int id);
        Task<IEnumerable<Folder>> GetAllAsync();
        Task<Folder> CreateAsync(Folder folder);
        Task<Folder> UpdateAsync(Folder folder);
        Task<bool> DeleteAsync(int id);
    }
}

