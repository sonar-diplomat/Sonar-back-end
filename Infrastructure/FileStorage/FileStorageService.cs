using System.Collections.Concurrent;
using Application.Abstractions.Interfaces.Service.File;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private static readonly ConcurrentDictionary<string, byte[]> _blobStorage = new();

    private readonly string _baseFolder;

    public FileStorageService()
    {
        _baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        DateTime now = DateTime.UtcNow;
        string folderPath = Path.Combine(subFolder, now.Year.ToString(), now.Month.ToString());

        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string blobKey = Path.Combine(folderPath, fileName).Replace("\\", "/");

        using (MemoryStream memoryStream = new())
        {
            await file.CopyToAsync(memoryStream);
            _blobStorage[blobKey] = memoryStream.ToArray();
        }

        return blobKey;
    }

    public byte[] GetFile(string blobKey)
    {
        if (_blobStorage.TryGetValue(blobKey, out byte[]? data))
            return data;

        throw new FileNotFoundException("File not found in blob storage", blobKey);
    }

    public bool DeleteFile(string blobKey)
    {
        return _blobStorage.TryRemove(blobKey, out _);
    }
}