using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class ThemeService(IThemeRepository repository) : GenericService<Theme>(repository), IThemeService
    {
        public async Task<Theme> GetDefaultAsync()
        {
            return await repository.GetDefaultAsync();
        }
    }
}

