using Application.Abstractions.Interfaces.Services;
using FileModel = Entities.Models.File.File;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IFileService : IGenericService<FileModel>
    {

    }
}

