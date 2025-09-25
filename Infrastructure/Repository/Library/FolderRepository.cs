using Application.Abstractions.Interfaces.Repository.Library;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Library
{
    public class FolderRepository : GenericRepository<Entities.Models.Folder>, IFolderRepository
    {
        public FolderRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
