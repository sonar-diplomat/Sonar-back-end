using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services;
using FileModel = Entities.Models.File.File;

namespace Application.Services.File;

public class FileService(IFileRepository repository) : GenericService<FileModel>(repository), IFileService
{
    public async Task<FileModel> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }
}
