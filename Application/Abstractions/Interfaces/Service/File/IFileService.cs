using File = Entities.Models.File;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IFileService
    {
        Task<File.File> GetByIdAsync(int id);
        Task<IEnumerable<File.File>> GetAllAsync();
        Task<File.File> CreateAsync(File.File file);
        Task<File.File> UpdateAsync(File.File file);
        Task<bool> DeleteAsync(int id);
    }
}

