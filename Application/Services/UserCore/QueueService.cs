using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.UserCore;

public class QueueService(
    IQueueRepository repository,
    ITrackService trackService
)
    : GenericService<Queue>(repository), IQueueService
{
    public async Task AddTrackToQueueAsync(int queueId, int trackId)
    {
        Queue queue = await GetQueueWithTracksAsync(queueId);
        Track track = await trackService.GetByIdValidatedAsync(trackId);
        int maxOrder = queue.QueueTracks.Any() ? queue.QueueTracks.Max(qt => qt.Order) : -1;
        QueueTrack queueTrack = new()
        {
            QueueId = queueId,
            TrackId = trackId,
            Order = maxOrder + 1,
            IsManuallyAdded = true
        };
        queue.QueueTracks.Add(queueTrack);
        await repository.UpdateAsync(queue);
    }

    public async Task RemoveTrackFromQueueAsync(int queueId, int trackId)
    {
        Queue queue = await GetQueueWithTracksAsync(queueId);
        QueueTrack? queueTrack = queue.QueueTracks.FirstOrDefault(qt => qt.TrackId == trackId);
        if (queueTrack != null)
        {
            queue.QueueTracks.Remove(queueTrack);
            List<QueueTrack> remainingTracks = queue.QueueTracks
                .Where(qt => qt.Order > queueTrack.Order)
                .OrderBy(qt => qt.Order)
                .ToList();
            foreach (var qt in remainingTracks)
            {
                qt.Order--;
            }
            await repository.UpdateAsync(queue);
        }
    }

    public async Task<Queue> GetQueueWithTracksAsync(int queueId)
    {
        return await repository
            .SnInclude(q => q.QueueTracks.OrderBy(qt => qt.Order))
            .ThenInclude(qt => qt.Track)
            .ThenInclude(t => t.TrackArtists)
            .Include(q => q.QueueTracks)
            .ThenInclude(qt => qt.Track.Genre)
            .Include(q => q.CurrentTrack)
            .Include(q => q.Collection)
            .GetByIdValidatedAsync(queueId);
    }

    public async Task SaveQueueAsync(int queueId, IEnumerable<int> trackIds)
    {
        Queue queue = await GetQueueWithTracksAsync(queueId);
        queue.QueueTracks.Clear();
        int order = 0;
        foreach (int trackId in trackIds)
        {
            Track track = await trackService.GetByIdValidatedAsync(trackId);
            QueueTrack queueTrack = new()
            {
                QueueId = queueId,
                TrackId = trackId,
                Order = order++,
                IsManuallyAdded = false
            };
            queue.QueueTracks.Add(queueTrack);
        }
        await repository.UpdateAsync(queue);
    }
}