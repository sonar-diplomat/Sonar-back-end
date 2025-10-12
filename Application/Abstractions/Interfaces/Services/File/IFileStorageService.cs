using Entities.Models.File;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services.File;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, FileType fileType);
    Task<byte[]> GetFile(string blobKey);
    Task<bool> DeleteFile(string blobKey);
}
