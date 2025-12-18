using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Application.Response;
using Entities.Models.Music;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserStateService(
    IUserStateRepository repository,
    IUserStatusService userStatusService,
    IUserSessionService userSessionService,
    ITrackService trackService,
    ICollectionService<Playlist> playlistCollectionService,
    ICollectionService<Album> albumCollectionService,
    ICollectionService<Blend> blendCollectionService,
    IQueueService queueService)
    : GenericService<UserState>(repository), IUserStateService
{
    public async Task<UserState> CreateDefaultAsync()
    {
        Queue queue = new()
        {
            Position = TimeSpan.Zero
        };
        await queueService.CreateAsync(queue);
        UserState result = new()
        {
            UserStatusId = 1,
            QueueId = queue.Id
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
        UserState userState = await repository.SnInclude(q => q.Queue).GetByIdValidatedAsync(stateId);;
        userState.Queue.Position = position;
        await repository.UpdateAsync(userState);
    }

    public async Task UpdatePrimarySessionAsync(int userId, string deviceId)
    {
        UserSession userSession = await userSessionService.GetByUserIdAndDeviceIdValidatedAsync(userId, deviceId);
        UserState userState = await GetByIdValidatedAsync(userSession.UserId);
        userState.PrimarySessionId = userSession.Id;
        await repository.UpdateAsync(userState);
    }

    public async Task UpdateListeningTargetAsync(int stateId, int trackId, int? collectionId)
    {
        UserState userState = await repository.SnInclude(us => us.Queue).GetByIdValidatedAsync(stateId);
        await trackService.GetByIdValidatedAsync(trackId);
        userState.Queue!.CurrentTrackId = trackId;
        if (collectionId == null)
            return;
        int id = (int)collectionId;
        // TODO: 😭😭😿😭😭  
        Collection? col = (await playlistCollectionService.GetByIdAsync(id) ??
                           (Collection)await albumCollectionService.GetByIdAsync(id)) ??
                          await blendCollectionService.GetByIdAsync(id);
        // TODO: 🥺
        if (col == null)
            throw ResponseFactory.Create<BadRequestResponse>([$"Collection with Id {id} not found."]);
        userState.Queue.CollectionId = id;
        await repository.UpdateAsync(userState);
    }

    public async Task<UserState> GetByUserIdValidatedAsync(int userId)
    {
        UserState? userState = await repository.GetByUserIdAsync(userId);
        return userState ??
               throw ResponseFactory.Create<BadRequestResponse>([$"UserState for User with Id {userId} not found."]);
    }

    public async Task AddTrackToUserQueueAsync(int userId, int trackId)
    {
        UserState userState = await GetByUserIdValidatedAsync(userId);
        await queueService.AddTrackToQueueAsync(userState.QueueId, trackId);
    }

    public async Task RemoveTrackFromUserQueueAsync(int userId, int trackId)
    {
        UserState userState = await GetByUserIdValidatedAsync(userId);
        await queueService.RemoveTrackFromQueueAsync(userState.QueueId, trackId);
    }

    public async Task<Queue> GetUserQueueAsync(int userId)
    {
        UserState userState = await GetByUserIdValidatedAsync(userId);
        return await queueService.GetQueueWithTracksAsync(userState.QueueId);
    }

    public async Task SaveUserQueueAsync(int userId, IEnumerable<int> trackIds)
    {
        UserState userState = await GetByUserIdValidatedAsync(userId);
        await queueService.SaveQueueAsync(userState.QueueId, trackIds);
    }
}