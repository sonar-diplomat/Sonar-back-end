using Entities.Models.File;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services.File;

public interface IAudioFileService : IFileService<AudioFile>
{
    Task<FileStream?> GetMusicStreamAsync(int fileId, long? startByte, long? length);
    Task<AudioFile> UploadFileAsync(IFormFile file);
    Task<TimeSpan> GetDurationAsync(IFormFile file);
}