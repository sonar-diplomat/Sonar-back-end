using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserStateService : IGenericService<UserState>
{
    Task<UserState> CreateDefaultAsync();
    Task UpdateUserStatusAsync(int stateId, int statusId);
    Task UpdateCurrentPositionAsync(int stateId, TimeSpan position);
    Task UpdatePrimarySessionAsync(int userId, string deviceId);
    Task UpdateListeningTargetAsync(int stateId, int trackId, int? collectionId);
    Task<UserState> GetByUserIdValidatedAsync(int userId);
    Task AddTracksToUserQueueAsync(int userId, IEnumerable<int> trackIds);
    Task RemoveTracksFromUserQueueAsync(int userId, IEnumerable<int> trackIds);
    Task<Queue> GetUserQueueAsync(int userId);
}