using Entities.Models.File;

namespace Application.Abstractions.Interfaces.Repository.File;

public interface IFileTypeRepository : IGenericRepository<FileType>
{
    Task<FileType?> GetByNameAsync(string name);
}
