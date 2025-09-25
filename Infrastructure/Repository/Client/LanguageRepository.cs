using Application.Abstractions.Interfaces.Repository.Client;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class LanguageRepository : GenericRepository<Entities.Models.Language>, ILanguageRepository
    {
        public LanguageRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
