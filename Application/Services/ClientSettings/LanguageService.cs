using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class LanguageService(ILanguageRepository repository) : GenericService<Language>(repository), ILanguageService
    {
        public async Task<Language> GetDefault()
        {
            return await repository.GetDefault();
        }
    }
}

