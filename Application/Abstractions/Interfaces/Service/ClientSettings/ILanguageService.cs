using Application.Abstractions.Interfaces.Service;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services;

public interface ILanguageService : IGenericService<Language>
{
    Task<Language> GetByLocaleAsync(string languageLocale);
}