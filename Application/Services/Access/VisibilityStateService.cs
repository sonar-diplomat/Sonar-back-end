using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Access;

namespace Application.Services.Access;

public class VisibilityStateService(
    IVisibilityStateRepository repository,
    IVisibilityStatusService visibilityStatusService
)
    : GenericService<VisibilityState>(repository), IVisibilityStateService
{
    public async Task<VisibilityState> CreateDefaultAsync(DateTime? setPublicOn = null)
    {
        VisibilityState defaultState = new()
        {
            SetPublicOn = setPublicOn ?? DateTime.MaxValue,
            StatusId = (await visibilityStatusService.GetDefaultAsync()).Id
        };

        return await CreateAsync(defaultState);
    }
}