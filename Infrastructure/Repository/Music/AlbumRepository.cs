using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models.Music;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Music;

public class AlbumRepository(SonarContext dbContext) : GenericRepository<Album>(dbContext), IAlbumRepository
{
    public async Task<IEnumerable<Album>> GetByDistributorIdAsync(int distributorId)
    {
        return await Query()
            .Include(a => a.Distributor)
            .Include(a => a.Tracks)
            .Include(a => a.AlbumArtists)
            .ThenInclude(aa => aa.Artist)
            .Where(a => a.DistributorId == distributorId)
            .OrderByDescending(a => a.Id)
            .ToListAsync();
    }

    public async Task<List<Track>> GetTracksFromAlbumAsync(int albumId)
    {
        IQueryable<Track> query = context.Albums
            .Where(a => a.Id == albumId)
            .SelectMany(a => a.Tracks)
            .Include(t => t.Cover)
            .Include(t => t.LowQualityAudioFile)
            .Include(t => t.TrackArtists)
            .ThenInclude(ta => ta.Artist)
            .ThenInclude(a => a.User)
            .Include(t => t.VisibilityState)
            .ThenInclude(vs => vs.Status)
            .AsQueryable();

        return await query
            .OrderBy(t => t.Id)
            .ToListAsync();
    }
}