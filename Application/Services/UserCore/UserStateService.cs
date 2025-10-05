using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserStateService(IUserStateRepository repository, IGenericRepository<UserState> genericRepository)
    : GenericService<UserState>(genericRepository), IUserStateService
{
    public async Task<UserState> CreateDefaultAsync()
    {
        UserState result = new()
        {
            UserStatusId = 1 // online
        };
        return await genericRepository.AddAsync(result);
    }
}