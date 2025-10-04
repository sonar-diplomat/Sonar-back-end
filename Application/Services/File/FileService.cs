using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services;

namespace Application.Services.File
{
    public class FileService(IFileRepository repository, IGenericRepository<Entities.Models.File.File> genericRepository) : GenericService<Entities.Models.File.File>(genericRepository), IFileService
    {

    }
}

