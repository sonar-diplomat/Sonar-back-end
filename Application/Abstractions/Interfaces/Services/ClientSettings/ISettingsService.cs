using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services;

public interface ISettingsService : IGenericService<Settings>
{
    Task<Settings> CreateDefaultAsync(string languageLocale);
}