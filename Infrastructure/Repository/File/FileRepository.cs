using Application.Abstractions.Interfaces.Repository.File;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.File;

public class FileRepository(SonarContext dbContext)
    : GenericRepository<Entities.Models.File.File>(dbContext), IFileRepository
{
    public async Task<Entities.Models.File.File> GetDefaultAsync()
    {
        return (await context.Files.FirstOrDefaultAsync(f => f.Id == 1))!;
    }
}
