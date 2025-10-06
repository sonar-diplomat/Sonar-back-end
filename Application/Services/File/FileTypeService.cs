using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.File;

namespace Application.Services.File;

public class FileTypeService(IFileTypeRepository repository) : GenericService<FileType>(repository), IFileTypeService
{
}