using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Service.File
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder);

        byte[] GetFile(string blobKey);

        bool DeleteFile(string blobKey);
    }
}