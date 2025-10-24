using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services.File;
using Entities.Models.File;
using FileSignatures;
using Microsoft.AspNetCore.Http;

namespace Application.Services.File;

public class ImageFileService(
    IImageFileRepository repository,
    IFileStorageService fileStorageService,
    IFileStorageService fileStorage,
    IFileFormatInspector inspector
) : FileService<ImageFile>(repository, fileStorage, inspector), IImageFileService
{
    public async Task<ImageFile> UploadFileAsync(IFormFile file)
    {
        //await ValidateFileType(file, [typeof(Png), typeof(Jpeg)]);
        //TODO:
        string url = await fileStorageService.SaveImageFileAsync(file);
        ImageFile fileModel = new()
        {
            ItemName = file.FileName,
            Url = url
        };
        return await CreateAsync(fileModel);
    }

    public async Task<ImageFile> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }
}