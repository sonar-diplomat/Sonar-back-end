using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private static readonly ConcurrentDictionary<string, byte[]> blobStorage = new();
    private readonly string baseFolder = Path.Combine("wwwroot", "uploads");

    /// <summary>
    /// Gets the full physical path for a relative file path.
    /// </summary>
    private string GetPhysicalPath(string relativePath)
    {
        // Normalize path separators for the current OS
        string normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar)
                                           .Replace('\\', Path.DirectorySeparatorChar);
        return Path.Combine(Directory.GetCurrentDirectory(), baseFolder, normalizedPath);
    }

    /// <summary>
    /// Normalizes a path to use forward slashes for URL compatibility.
    /// </summary>
    private string NormalizePathForUrl(string path)
    {
        return path.Replace('\\', '/');
    }

    public async Task<byte[]> GetFile(string relativePath)
    {
        // Normalize the relative path (remove leading slashes)
        string normalizedRelativePath = relativePath.TrimStart('/', '\\');
        
        if (blobStorage.TryGetValue(normalizedRelativePath, out byte[]? data))
        {
            return data;
        }

        string physicalPath = GetPhysicalPath(normalizedRelativePath);
        
        if (File.Exists(physicalPath))
        {
            data = await File.ReadAllBytesAsync(physicalPath);
            blobStorage[normalizedRelativePath] = data;
            return data;
        }

        throw ResponseFactory.Create<NotFoundResponse>([$"File not found with path '{normalizedRelativePath}'"]);
    }

    public async Task<bool> DeleteFile(string relativePath)
    {
        // Normalize the relative path (remove leading slashes)
        string normalizedRelativePath = relativePath.TrimStart('/', '\\');
        
        bool removedFromMemory = blobStorage.TryRemove(normalizedRelativePath, out _);

        string physicalPath = GetPhysicalPath(normalizedRelativePath);
        bool removedFromDisk = false;
        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
            removedFromDisk = true;
        }

        return removedFromMemory || removedFromDisk;
    }

    public async Task<string> SaveAudioFileAsync(IFormFile file)
    {
        return await SaveFileAsync(file, "audio");
    }

    public async Task<string> SaveImageFileAsync(IFormFile file)
    {
        return await SaveFileAsync(file, "image");
    }

    public async Task<string> SaveVideoFileAsync(IFormFile file)
    {
        return await SaveFileAsync(file, "video");
    }

    private async Task<string> SaveFileAsync(IFormFile file, string category)
    {
        if (file == null)
            throw ResponseFactory.Create<BadRequestResponse>(["File not found"]);

        DateTime now = DateTime.UtcNow;
        
        // Build relative path components
        string year = now.Year.ToString();
        string month = now.Month.ToString();
        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        
        // Create relative path with forward slashes for URL compatibility
        string relativePath = NormalizePathForUrl(Path.Combine(category, year, month, fileName));
        
        // Get physical path for file system operations
        string physicalPath = GetPhysicalPath(relativePath);
        
        // Ensure directory exists
        string? directory = Path.GetDirectoryName(physicalPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Save file
        using MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);
        byte[] fileData = memoryStream.ToArray();
        
        // Store in memory cache
        blobStorage[relativePath] = fileData;
        
        // Write to disk
        await File.WriteAllBytesAsync(physicalPath, fileData);

        // Return relative path (with forward slashes for URL compatibility)
        return relativePath;
    }
}