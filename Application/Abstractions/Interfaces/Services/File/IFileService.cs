using Entities.Models.File;
using Microsoft.AspNetCore.Http;
using FileModel = Entities.Models.File.File;

namespace Application.Abstractions.Interfaces.Services;

public interface IFileService : IGenericService<FileModel>
{
    Task<FileStream> GetMusicStreamAsync(int trackId, long? startByte, long? length);
    Task<FileModel> GetDefaultAsync();
    Task<FileModel> UploadFileAsync(FileType fileType, IFormFile file);
}
