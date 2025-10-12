using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models.File;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.File;

public class FileTypeRepository(SonarContext dbContext) : GenericRepository<FileType>(dbContext), IFileTypeRepository
{
    public async Task<FileType?> GetByNameAsync(string name)
    {
        return await context.FileTypes.FirstOrDefaultAsync(ft => ft.Name == name);
    }
}
