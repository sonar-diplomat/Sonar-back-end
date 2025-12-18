using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models.File;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.File;

public class ImageFileRepository(SonarContext dbContext)
    : GenericRepository<ImageFile>(dbContext), IImageFileRepository
{
    public async Task<ImageFile> GetDefaultAsync()
    {
        return (await context.ImageFiles.FirstOrDefaultAsync(f => f.Id == 1))!;
    }

    public async Task<ImageFile> GetFavoriteDefaultAsync()
    {
        return (await context.ImageFiles.FirstOrDefaultAsync(f => f.Id == 6))!;
    }

    public async Task<ImageFile> GetPlaylistDefaultAsync()
    {
        return (await context.ImageFiles.FirstOrDefaultAsync(f => f.Id == 2))!;
    }
}