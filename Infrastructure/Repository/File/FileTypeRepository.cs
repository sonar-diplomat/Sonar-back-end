using Application.Abstractions.Interfaces.Repository.File;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.File
{
    public class FileTypeRepository : GenericRepository<Entities.Models.FileType>, IFileTypeRepository
    {
        public FileTypeRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
