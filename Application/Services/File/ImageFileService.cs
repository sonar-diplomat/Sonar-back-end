using System.IO;
using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Entities.Models.File;
using FileSignatures;
using FileSignatures.Formats;
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
        await ValidateFileType(file, [typeof(Png), typeof(Jpeg)]);
        string? url = await fileStorageService.SaveImageFileAsync(file);

        if (url == null) throw ResponseFactory.Create<InternalServerErrorResponse>(["Can`t save image"]);

        string extension = Path.GetExtension(file.FileName);
        string itemName = $"{Guid.NewGuid()}{extension}";

        ImageFile fileModel = new()
        {
            ItemName = itemName,
            Url = url
        };
        return await CreateAsync(fileModel);
    }

    public async Task<ImageFile> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }

    public Task<ImageFile> GetFavoriteDefaultAsync()
    {
        return repository.GetFavoriteDefaultAsync();
    }

    public Task<ImageFile> GetPlaylistDefaultAsync()
    {
        return repository.GetPlaylistDefaultAsync();
    }

    public async Task<ImageFile?> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<ImageFile> GetByIdValidatedAsync(int id)
    {
        ImageFile? entity = await repository.GetByIdAsync(id);
        return entity ?? throw ResponseFactory.Create<NotFoundResponse>([$"ImageFile not found"]);
    }
}