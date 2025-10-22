using FileSignatures;
using Microsoft.AspNetCore.Http;
using FileModel = Entities.Models.File.File;

namespace Application.Abstractions.Interfaces.Services;

public interface IFileService<T> where T : FileModel
{
    Task DeleteAsync(int id);
    Task<FileFormat> ValidateFileType(IFormFile file, Type[] permittedFormats);
}