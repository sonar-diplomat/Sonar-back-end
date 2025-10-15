using System.Collections.Concurrent;
using Application.Abstractions.Interfaces.Services.File;
using Application.Exception;
using Entities.Models.File;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private static readonly ConcurrentDictionary<string, byte[]> blobStorage = new();
    private readonly string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public async Task<byte[]> GetFile(string blobKey)
    {
        return blobStorage.TryGetValue(blobKey, out byte[]? data) ? data :
            throw AppExceptionFactory.Create<NotFoundException>([$"File not found in blob storage with key '{blobKey}'"]);
    }

    public async Task<bool> DeleteFile(string blobKey)
    {
        return blobStorage.TryRemove(blobKey, out _);
    }

    public async Task<string> SaveFileAsync(IFormFile file, FileType fileType)
    {
        if (file == null)
            throw AppExceptionFactory.Create<BadRequestException>(["File not found"]);

        DateTime now = DateTime.UtcNow;
        string folderPath = Path.Combine(baseFolder, fileType.Name, now.Year.ToString(), now.Month.ToString());

        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string blobKey = Path.Combine(folderPath, fileName).Replace("\\", "/");

        using MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);
        blobStorage[blobKey] = memoryStream.ToArray();

        return blobKey;
    }
}
