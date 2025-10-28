using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private static readonly ConcurrentDictionary<string, byte[]> blobStorage = new();
    private readonly string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public async Task<byte[]> GetFile(string blobKey)
    {
        if (blobStorage.TryGetValue(blobKey, out byte[]? data))
        {
            return data;
        }

        if (File.Exists(blobKey))
        {
            data = await File.ReadAllBytesAsync(blobKey);
            blobStorage[blobKey] = data;
            return data;
        }

        throw ResponseFactory.Create<NotFoundResponse>([$"File not found with key '{blobKey}'"]);
    }

    public async Task<bool> DeleteFile(string blobKey)
    {
        bool removedFromMemory = blobStorage.TryRemove(blobKey, out _);

        bool removedFromDisk = false;
        if (File.Exists(blobKey))
        {
            File.Delete(blobKey);
            removedFromDisk = true;
        }

        return removedFromMemory || removedFromDisk;
    }

    public async Task<string> SaveAudioFileAsync(IFormFile file)
    {
        string path = Path.Combine(baseFolder, "audio");
        return await SaveFileAsync(file, path);
    }

    public async Task<string> SaveImageFileAsync(IFormFile file)
    {
        string path = Path.Combine(baseFolder, "image");
        return await SaveFileAsync(file, path);
    }

    public async Task<string> SaveVideoFileAsync(IFormFile file)
    {
        string path = Path.Combine(baseFolder, "video");
        return await SaveFileAsync(file, path);
    }

    private async Task<string> SaveFileAsync(IFormFile file, string baseUrl)
    {
        if (file == null)
            throw ResponseFactory.Create<BadRequestResponse>(["File not found"]);



        DateTime now = DateTime.UtcNow;
        string folderPath = Path.Combine(baseUrl, now.Year.ToString(), now.Month.ToString());

        Directory.CreateDirectory(folderPath);

        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string blobKey = Path.Combine(folderPath, fileName).Replace("\\", "/");

        using MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);
        blobStorage[blobKey] = memoryStream.ToArray();

        await File.WriteAllBytesAsync(blobKey, memoryStream.ToArray());

        return blobKey;
    }
}