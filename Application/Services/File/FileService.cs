using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Exception;
using Entities.Models.File;
using FileSignatures;
using FileSignatures.Formats;
using Microsoft.AspNetCore.Http;
using FileModel = Entities.Models.File.File;

namespace Application.Services.File;

public class FileService(
    IFileRepository repository,
    IFileStorageService fileStorageService,
    IFileTypeService fileTypeService,
    IFileFormatInspector inspector
) : GenericService<FileModel>(repository), IFileService
{
    private static readonly Dictionary<string, Type[]> allowedFormats = new()
    {
        { "Image", [typeof(Png), typeof(Jpeg)] },
        { "Audio", [typeof(Mp3), typeof(Flac)] },
        { "Gif", [typeof(Gif)] }
    };

    public async Task<FileModel> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }

    public async Task<FileModel> UploadFileAsync(FileType fileType, IFormFile file)
    {
        await ValidateFileType(fileType, file);
        string url = await fileStorageService.SaveFileAsync(file, fileType);

        FileModel fileModel = new()
        {
            ItemName = file.FileName,
            Url = url,
            Type = fileType
        };

        return await CreateAsync(fileModel);
    }

    public override async Task DeleteAsync(int id)
    {
        FileModel file = await GetByIdValidatedAsync(id);
        bool success = await fileStorageService.DeleteFile(file.Url);
        if (!success)
            throw AppExceptionFactory.Create<ConflictException>();
        await base.DeleteAsync(file);
    }

    public async Task ValidateFileType(FileType fileType, IFormFile file)
    {
        await using Stream? stream = file.OpenReadStream();
        FileFormat? format = inspector.DetermineFileFormat(stream);

        if (allowedFormats.TryGetValue(fileType.Name, out Type[]? formats))
            if (formats.Any(f => f == format?.GetType()))
                return;

        throw AppExceptionFactory.Create<BadRequestException>(["Invalid file type"]);
    }
}

public class Mp3 : FileFormat
{
    public Mp3() : base("ID3"u8.ToArray(), "audio/mpeg", "mp3")
    {
    }
}

public class Flac : FileFormat
{
    public Flac() : base("fLaC"u8.ToArray(), "audio/flac", "flac")
    {
    }
}
