using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserPrivacyGroupService(IUserPrivacyGroupRepository repository)
    : GenericService<UserPrivacyGroup>(repository), IUserPrivacyGroupService
{
    public async Task<UserPrivacyGroup> GetDefaultAsync()
    {
        return await repository.GetDefaultAsync();
    }
}