using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IQueueService : IGenericService<Queue>
{
    Task AddTrackToQueueAsync(int queueId, int trackId);
    Task RemoveTrackFromQueueAsync(int queueId, int trackId);
    Task<Queue> GetQueueWithTracksAsync(int queueId);
    Task SaveQueueAsync(int queueId, IEnumerable<int> trackIds);
}