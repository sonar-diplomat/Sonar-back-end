using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services.File;
using Entities.Models.File;
using FileSignatures;
using FileSignatures.Formats;
using Microsoft.AspNetCore.Http;

namespace Application.Services.File;

public class VideoFileService(
    IVideoFileRepository repository,
    IFileStorageService fileStorageService,
    IFileStorageService fileStorage,
    IFileFormatInspector inspector
) : FileService<VideoFile>(repository, fileStorage, inspector), IVideoFileService
{
    public async Task<VideoFile> UploadFileAsync(IFormFile file)
    {
        await ValidateFileType(file, [typeof(Gif)]);
        string url = await fileStorageService.SaveVideoFileAsync(file);
        VideoFile fileModel = new()
        {
            ItemName = file.FileName,
            Url = url
        };
        return await CreateAsync(fileModel);
    }
}