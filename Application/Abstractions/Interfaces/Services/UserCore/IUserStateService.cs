using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserStateService : IGenericService<UserState>
{
    Task<UserState> CreateDefaultAsync();
    Task UpdateUserStatusAsync(int stateId, int statusId);
    Task UpdateCurrentPositionAsync(int stateId, TimeSpan position);
}