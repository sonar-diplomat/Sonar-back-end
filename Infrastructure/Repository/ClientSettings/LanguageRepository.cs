using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Client
{
    public class LanguageRepository(SonarContext dbContext) : GenericRepository<Language>(dbContext), ILanguageRepository
    {
        public async Task<Language> GetByLocaleAsync(string locale)
        {
            return (await context.Languages.FirstOrDefaultAsync(l => l.Locale == locale))!;
        }

    }
}
