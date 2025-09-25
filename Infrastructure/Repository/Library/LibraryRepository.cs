using Application.Abstractions.Interfaces.Repository.Library;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Library
{
    public class LibraryRepository : GenericRepository<Entities.Models.Library.Library>, ILibraryRepository
    {
        public LibraryRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
