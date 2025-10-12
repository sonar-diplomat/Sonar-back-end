using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services;
using Application.Exception;
using Entities.Models.File;

namespace Application.Services.File;

public class FileTypeService(IFileTypeRepository repository) : GenericService<FileType>(repository), IFileTypeService
{
    public async Task<FileType> GetByNameAsync(string name)
    {
        FileType? fileType = await repository.GetByNameAsync(name);
        return fileType ?? throw AppExceptionFactory.Create<NotFoundException>([$"File type with name '{name}' not found"]);
    }
}
