using System.Text.Json;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services;

public interface ISettingsService : IGenericService<Settings>
{
    Task<Settings> CreateDefaultAsync(string languageLocale);
    Task<Settings> GetByIdValidatedFullAsync(int userSettingsId);
    Task<Settings> PatchAsync(int settingsId, JsonElement updates);
}