using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Repository.Client;

public interface ILanguageRepository : IGenericRepository<Language>
{
    Task<Language> GetByLocaleAsync(string languageLocale);
}