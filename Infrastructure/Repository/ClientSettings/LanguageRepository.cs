using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
