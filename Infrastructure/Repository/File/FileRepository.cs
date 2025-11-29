using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models.File;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;
using FileModel = Entities.Models.File.File;

namespace Infrastructure.Repository.File;

public class FileRepository(SonarContext dbContext)
    : GenericRepository<FileModel>(dbContext), IFileRepository
{
    public async Task<FileModel> GetDefaultAsync()
    {
        return (await context.Set<FileModel>().FirstOrDefaultAsync(f => f.Id == 1))!;
    }
}

