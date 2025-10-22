using Entities.Models.File;

namespace Application.Abstractions.Interfaces.Services.File;

public interface IAudioFileService : IFileService<AudioFile>
{
    Task<FileStream?> GetMusicStreamAsync(int fileId, long? startByte, long? length);
}