using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using FileSignatures;
using FileSignatures.Formats;
using Microsoft.AspNetCore.Http;
using FileModel = Entities.Models.File.File;

namespace Application.Services.File;

public abstract class FileService<T>(
    IGenericRepository<T> repository,
    IFileStorageService fileStorageService,
    IFileFormatInspector inspector
) : GenericService<T>(repository), IFileService<T> where T : FileModel
{
    private static readonly Type[] allowedFormats = [typeof(Png), typeof(Jpeg), typeof(Mp3), typeof(Flac), typeof(Gif)];

    public override async Task DeleteAsync(int id)
    {
        T file = await GetByIdValidatedAsync(id);
        bool success = await fileStorageService.DeleteFile(file.Url);
        if (!success)
            throw ResponseFactory.Create<ConflictResponse>();
        await base.DeleteAsync(file);
    }

    public async Task<FileFormat> ValidateFileType(IFormFile file, Type[]? formats = null)
    {
        await using Stream stream = file.OpenReadStream();
        FileFormat? format = inspector.DetermineFileFormat(stream);
        Type[] permittedFormats = formats ?? allowedFormats;
        if (format != null && permittedFormats.Contains(format.GetType()))
            return format;
        throw ResponseFactory.Create<BadRequestResponse>(["Invalid file type"]);
    }
}

public class Mp3() : FileFormat("ID3"u8.ToArray(), "audio/mpeg", "mp3");

public class Flac() : FileFormat("fLaC"u8.ToArray(), "audio/flac", "flac");