using Entities.Models.File;
using Microsoft.AspNetCore.Http;
using FileModel = Entities.Models.File.File;

namespace Application.Abstractions.Interfaces.Services;

public interface IFileService : IGenericService<FileModel>
{
    Task<FileModel> GetDefaultAsync();
    Task<FileModel> UploadFileAsync(FileType fileType, IFormFile file);
}
