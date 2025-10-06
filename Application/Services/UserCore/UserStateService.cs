using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserStateService(IUserStateRepository repository)
    : GenericService<UserState>(repository), IUserStateService
{
    public async Task<UserState> CreateDefaultAsync()
    {
        UserState result = new()
        {
            UserStatusId = 1 // online
        };
        return await repository.AddAsync(result);
    }
}