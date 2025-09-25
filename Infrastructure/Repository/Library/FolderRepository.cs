using Application.Abstractions.Interfaces.Repository.Library;
using Entities.Models.Library;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Library
{
    public class FolderRepository : GenericRepository<Folder>, IFolderRepository
    {
        public FolderRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
