using Entities.Models.File;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services.File;

public interface IImageFileService : IFileService<ImageFile>
{
    Task<ImageFile> UploadFileAsync(IFormFile file);
    Task<ImageFile> GetDefaultAsync();
    Task<ImageFile> GetFavoriteDefaultAsync();
    Task<ImageFile> GetPlaylistDefaultAsync();
}