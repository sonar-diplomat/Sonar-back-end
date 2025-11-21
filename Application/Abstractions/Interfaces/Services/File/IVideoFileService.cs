using Entities.Models.File;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services.File;

public interface IVideoFileService : IFileService<VideoFile>
{
    Task<VideoFile> UploadFileAsync(IFormFile file);
}