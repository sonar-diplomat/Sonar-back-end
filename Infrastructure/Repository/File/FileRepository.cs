using Application.Abstractions.Interfaces.Repository.File;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.File
{
    public class FileRepository : GenericRepository<Entities.Models.File.File>, IFileRepository
    {
        public FileRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
