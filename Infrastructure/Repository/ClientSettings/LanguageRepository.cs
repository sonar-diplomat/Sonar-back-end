using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.ClientSettings;

public class LanguageRepository(SonarContext dbContext) : GenericRepository<Language>(dbContext), ILanguageRepository
{
    public async Task<Language> GetByLocaleAsync(string locale)
    {
        Language? lang = await context.Languages.FirstOrDefaultAsync(l => l.Locale == locale);
        return lang ?? context.Languages.First(l => l.Id == 1);
    }
}
