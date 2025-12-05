using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models.Music;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Music;

public class PlaylistRepository(SonarContext dbContext) : GenericRepository<Playlist>(dbContext), IPlaylistRepository
{
    public async Task<List<Track>> GetTracksFromPlaylistAfterAsync(int playlistId, int? afterId, int limit)
    {
        IQueryable<Track> query = context.Playlists
            .Where(p => p.Id == playlistId)
            .SelectMany(p => p.Tracks)
            .Include(t => t.Cover)
            .Include(t => t.LowQualityAudioFile) // TODO: Get audio file based on user's ClientSettings
            .Include(t => t.TrackArtists)
            .ThenInclude(ta => ta.Artist)
            .ThenInclude(a => a.User)
            .Include(t => t.VisibilityState)
            .ThenInclude(vs => vs.Status)
            .AsQueryable();

        if (afterId.HasValue)
            query = query.Where(t => t.Id > afterId.Value);

        return await query
            .OrderBy(t => t.Id)
            .Take(limit)
            .ToListAsync();
    }
}