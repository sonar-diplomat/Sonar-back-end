using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services.File;

public interface IFileStorageService
{
    Task<string> SaveAudioFileAsync(IFormFile file);
    Task<string> SaveImageFileAsync(IFormFile file);
    Task<string> SaveVideoFileAsync(IFormFile file);
    Task<byte[]> GetFile(string blobKey);
    Task<bool> DeleteFile(string blobKey);
}