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
    public async Task AddTracksToQueueAsync(int queueId, IEnumerable<int> trackIds)
    {
        Queue queue = await GetQueueWithTracksAsync(queueId);
        List<Track> tracksToAdd = new();
        foreach (int trackId in trackIds)
        {
            Track track = await trackService.GetByIdValidatedAsync(trackId);
            tracksToAdd.Add(track);
        }

        foreach (Track track in tracksToAdd)
        {
            if (queue.Tracks.All(t => t.Id != track.Id))
            {
                queue.Tracks.Add(track);
            }
        }

        await repository.UpdateAsync(queue);
    }

    public async Task RemoveTracksFromQueueAsync(int queueId, IEnumerable<int> trackIds)
    {
        Queue queue = await GetQueueWithTracksAsync(queueId);
        List<Track> tracksToRemove = queue.Tracks.Where(t => trackIds.Contains(t.Id)).ToList();
        
        foreach (Track track in tracksToRemove)
        {
            queue.Tracks.Remove(track);
        }

        await repository.UpdateAsync(queue);
    }

    public async Task<Queue> GetQueueWithTracksAsync(int queueId)
    {
        return await repository.SnInclude(q => q.Tracks)
            .ThenInclude(t => t.TrackArtists)
            .Include(q => q.CurrentTrack)
            .Include(q => q.Collection)
            .GetByIdValidatedAsync(queueId);
    }

    public async Task SaveQueueAsync(int queueId, IEnumerable<int> trackIds)
    {
        Queue queue = await GetQueueWithTracksAsync(queueId);
        queue.Tracks.Clear();
        List<Track> tracksToAdd = new();
        foreach (int trackId in trackIds)
        {
            Track track = await trackService.GetByIdValidatedAsync(trackId);
            tracksToAdd.Add(track);
        }

        foreach (Track track in tracksToAdd)
        {
            queue.Tracks.Add(track);
        }

        await repository.UpdateAsync(queue);
    }
}