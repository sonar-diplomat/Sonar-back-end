using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Access;

namespace Application.Services.Access
{
    public class AccessFeatureService(IAccessFeatureRepository repository) : GenericService<AccessFeature>(repository), IAccessFeatureService
    {
        public async Task<ICollection<AccessFeature>> GetDefaultAsync() => await repository.GetDefaultAsync();

    }
}

