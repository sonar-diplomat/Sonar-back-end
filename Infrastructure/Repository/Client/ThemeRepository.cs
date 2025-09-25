using Application.Abstractions.Interfaces.Repository.Client;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class ThemeRepository : GenericRepository<Entities.Models.Theme>, IThemeRepository
    {
        public ThemeRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
