using Entities.Models.File;

namespace Application.Abstractions.Interfaces.Services;

public interface IFileTypeService : IGenericService<FileType>
{
    Task<FileType> GetByNameAsync(string name);
}
