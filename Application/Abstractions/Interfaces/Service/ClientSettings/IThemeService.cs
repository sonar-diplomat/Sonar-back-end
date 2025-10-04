using Application.Abstractions.Interfaces.Service;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IThemeService : IGenericService<Theme>
    {
        Task<Theme> GetDefaultAsync();
    }
}

