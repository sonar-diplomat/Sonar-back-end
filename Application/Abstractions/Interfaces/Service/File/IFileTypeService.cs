using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.File;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IFileTypeService
    {
        Task<FileType> GetByIdAsync(int id);
        Task<IEnumerable<FileType>> GetAllAsync();
        Task<FileType> CreateAsync(FileType type);
        Task<FileType> UpdateAsync(FileType type);
        Task<bool> DeleteAsync(int id);
    }
}
