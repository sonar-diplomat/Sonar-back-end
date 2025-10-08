using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models.File;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.File;

public class FileTypeRepository : GenericRepository<FileType>, IFileTypeRepository
{
    public FileTypeRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
