using Application.Abstractions.Interfaces.Repository.Library;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Library
{
    public class LibraryRepository : GenericRepository<Entities.Models.Library.Library>, ILibraryRepository
    {
        public LibraryRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
