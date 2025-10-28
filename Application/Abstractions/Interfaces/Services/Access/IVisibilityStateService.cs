using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Services;

public interface IVisibilityStateService : IGenericService<VisibilityState>
{
    Task<VisibilityState> CreateDefaultAsync();
}
