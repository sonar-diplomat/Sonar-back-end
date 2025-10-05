using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Repository.Client;

public interface IThemeRepository : IGenericRepository<Theme>
{
    Task<Theme> GetDefaultAsync();
}