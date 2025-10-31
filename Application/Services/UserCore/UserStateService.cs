using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserStateService(IUserStateRepository repository, IUserStatusService userStatusService)
    : GenericService<UserState>(repository), IUserStateService
{
    public async Task<UserState> CreateDefaultAsync()
    {
        UserState result = new()
        {
            UserStatusId = 1 // online
            // TODO: Add default queue creation (queue service and repository)
        };
        return await repository.AddAsync(result);
    }

    public async Task UpdateUserStatusAsync(int stateId, int statusId)
    {
        UserState userState = await GetByIdValidatedAsync(stateId);
        await userStatusService.GetByIdValidatedAsync(statusId);
        userState.UserStatusId = statusId;
        await repository.UpdateAsync(userState);
    }

    public async Task UpdateCurrentPositionAsync(int stateId, TimeSpan position)
    {
        // TODO: Complete this shit 
        UserState userState = await repository.Include(q => q.Queue).GetByIdValidatedAsync<UserState>(stateId);
        userState.Queue.Position = position;
        await repository.UpdateAsync(userState);
    }
}