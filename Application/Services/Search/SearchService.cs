using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Music;
using Application.DTOs.Search;
using Application.Extensions;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Search;

public class SearchService(
    ITrackRepository trackRepository,
    IAlbumRepository albumRepository,
    IPlaylistRepository playlistRepository,
    IArtistRepository artistRepository,
    IUserRepository userRepository,
    IVisibilityStateService visibilityStateService)
    : ISearchService
{
    public async Task<SearchResultDTO> SearchAsync(string query, string? category = null, int limit = 20, int offset = 0, int? userId = null)
    {
        SearchResultDTO result = new()
        {
            Query = query,
            TotalResults = 0
        };

        string normalizedCategory = category?.ToLower() ?? "all";

        if (normalizedCategory is "all" or "tracks")
        {
            result.Tracks = await SearchTracksAsync(query, limit, offset, userId);
            result.TotalResults += result.Tracks.Total;
        }

        if (normalizedCategory is "all" or "albums")
        {
            result.Albums = await SearchAlbumsAsync(query, limit, offset, userId);
            result.TotalResults += result.Albums.Total;
        }

        if (normalizedCategory is "all" or "playlists")
        {
            result.Playlists = await SearchPlaylistsAsync(query, limit, offset, userId);
            result.TotalResults += result.Playlists.Total;
        }

        if (normalizedCategory is "all" or "artists" or "creators")
        {
            result.Artists = await SearchArtistsAsync(query, limit, offset, userId);
            result.TotalResults += result.Artists.Total;
        }

        if (normalizedCategory is "all" or "users" or "creators")
        {
            result.Users = await SearchUsersAsync(query, limit, offset, userId);
            result.TotalResults += result.Users.Total;
        }

        return result;
    }

    public async Task<SearchTracksResultDTO> SearchTracksAsync(string query, int limit = 20, int offset = 0, int? userId = null)
    {
        IQueryable<Track> tracks = await trackRepository.GetAllAsync();
        string searchPattern = $"{query}".ToLower().Trim();

        tracks = tracks
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .SnInclude(t => t.TrackArtists)
            .SnInclude(t => t.Collections)
            .Where(t =>
                t.Title.ToLower().Trim().Contains(searchPattern) ||
                t.TrackArtists.Any(ta => ta.Artist!.ArtistName.Contains(searchPattern))
            );

        // Фильтрация по видимости для треков
        tracks = FilterTracksByVisibility(tracks, userId);
        var names = tracks.ToList();
        int total = await tracks.CountAsync();

        List<Track> trackList = await tracks
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        List<TrackSearchItemDTO> items = trackList.Select(t =>
        {
            Album? album = t.Collections?.OfType<Album>().FirstOrDefault();
            return new TrackSearchItemDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationInSeconds = (int)(t.Duration?.TotalSeconds ?? 0),
                CoverId = t.CoverId,
                Artists = t.TrackArtists?.Select(ta => new AuthorDTO
                {
                    Pseudonym = ta.Pseudonym,
                    ArtistId = ta.ArtistId
                }) ?? new List<AuthorDTO>(),
                AlbumId = album?.Id,
                AlbumName = album?.Name
            };
        }).ToList();

        return new SearchTracksResultDTO
        {
            Total = total,
            Items = items
        };
    }

    public async Task<SearchAlbumsResultDTO> SearchAlbumsAsync(string query, int limit = 20, int offset = 0, int? userId = null)
    {
        IQueryable<Album> albums = await albumRepository.GetAllAsync();

        string searchPattern = $"{query}".ToLower().Trim();
        albums = albums
            .SnInclude(a => a.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .SnInclude(a => a.AlbumArtists)
            .SnInclude(a => a.Distributor)
            .SnInclude(a => a.Tracks)
            .Where(a =>
                a.Name.ToLower().Trim().Contains(searchPattern) ||
                a.AlbumArtists.Any(aa => aa.Artist!.ArtistName.Contains(searchPattern))
            );

        // Фильтрация по видимости
        albums = FilterByVisibility(albums, userId);

        int total = await albums.CountAsync();

        List<Album> albumList = await albums
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        List<AlbumSearchItemDTO> items = albumList.Select(a => new AlbumSearchItemDTO
        {
            Id = a.Id,
            Name = a.Name,
            CoverId = a.CoverId,
            TrackCount = a.Tracks?.Count ?? 0,
            Authors = a.AlbumArtists?.Select(aa => new AuthorDTO
            {
                Pseudonym = aa.Pseudonym,
                ArtistId = aa.ArtistId
            }) ?? new List<AuthorDTO>(),
            DistributorName = a.Distributor?.Name
        }).ToList();

        return new SearchAlbumsResultDTO
        {
            Total = total,
            Items = items
        };
    }

    public async Task<SearchPlaylistsResultDTO> SearchPlaylistsAsync(string query, int limit = 20, int offset = 0, int? userId = null)
    {
        IQueryable<Playlist> playlists = await playlistRepository.GetAllAsync();

        string searchPattern = $"{query}".ToLower().Trim();
        playlists = playlists
            .SnInclude(p => p.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .SnInclude(p => p.Creator)
            .SnInclude(p => p.Contributors)
            .SnInclude(p => p.Tracks)
            .Where(p =>
                p.Name.ToLower().Trim().Contains(searchPattern) ||
                (p.Creator != null && p.Creator.UserName != null && p.Creator.UserName.ToLower().Trim().Contains(searchPattern))
            );

        // Фильтрация по видимости
        playlists = FilterByVisibility(playlists, userId);

        int total = await playlists.CountAsync();

        List<Playlist> playlistList = await playlists
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        List<PlaylistSearchItemDTO> items = playlistList.Select(p => new PlaylistSearchItemDTO
        {
            Id = p.Id,
            Name = p.Name,
            CoverId = p.CoverId,
            TrackCount = p.Tracks?.Count ?? 0,
            CreatorName = p.Creator?.UserName ?? string.Empty,
            ContributorNames = p.Contributors?.Select(c => c.UserName ?? string.Empty).ToList() ?? new List<string>()
        }).ToList();

        return new SearchPlaylistsResultDTO
        {
            Total = total,
            Items = items
        };
    }

    public async Task<SearchArtistsResultDTO> SearchArtistsAsync(string query, int limit = 20, int offset = 0, int? userId = null)
    {
        IQueryable<Artist> artists = await artistRepository.GetAllAsync();

        string searchPattern = $"{query}".ToLower().Trim();
        artists = artists
            .SnInclude(a => a.User)
            .Where(a => a.ArtistName.ToLower().Trim().Contains(searchPattern));

        int total = await artists.CountAsync();

        List<Artist> artistList = await artists
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        // TODO: Подсчет треков и альбомов для каждого артиста
        List<ArtistSearchItemDTO> items = artistList.Select(a => new ArtistSearchItemDTO
        {
            Id = a.Id,
            ArtistName = a.ArtistName,
            UserId = a.UserId,
            AvatarImageId = a.User?.AvatarImageId ?? 0,
            TrackCount = 0, // TODO: Реализовать подсчет
            AlbumCount = 0  // TODO: Реализовать подсчет
        }).ToList();

        return new SearchArtistsResultDTO
        {
            Total = total,
            Items = items
        };
    }

    public async Task<SearchUsersResultDTO> SearchUsersAsync(string query, int limit = 20, int offset = 0, int? userId = null)
    {
        IQueryable<User> users = await userRepository.GetAllAsync();

        string searchPattern = $"%{query}%";
        users = users
            .Include(u => u.Artist)
            .Where(u =>
                (u.UserName != null && EF.Functions.Like(u.UserName, searchPattern)) ||
                EF.Functions.Like(u.PublicIdentifier, searchPattern)
            );

        int total = await users.CountAsync();

        List<User> userList = await users
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        List<UserSearchItemDTO> items = userList.Select(u => new UserSearchItemDTO
        {
            Id = u.Id,
            UserName = u.UserName ?? string.Empty,
            PublicIdentifier = u.PublicIdentifier,
            AvatarImageId = u.AvatarImageId,
            IsArtist = u.Artist != null,
            ArtistName = u.Artist?.ArtistName
        }).ToList();

        return new SearchUsersResultDTO
        {
            Total = total,
            Items = items
        };
    }

    public async Task<IEnumerable<string>> GetSuggestionsAsync(string query, int limit = 10)
    {
        // TODO: Реализовать автодополнение на основе популярных запросов или истории
        // Пока возвращаем пустой список
        return await Task.FromResult(Enumerable.Empty<string>());
    }

    public async Task<IEnumerable<string>> GetPopularQueriesAsync(int limit = 10)
    {
        // TODO: Реализовать получение популярных запросов из базы данных или кэша
        // Пока возвращаем пустой список
        return await Task.FromResult(Enumerable.Empty<string>());
    }

    private IQueryable<T> FilterByVisibility<T>(IQueryable<T> query, int? userId) where T : Collection
    {
        // Используем ShouldAppearInSearch для фильтрации результатов поиска
        // Для поиска показываем только Visible и Restricted контент, который уже публичен
        return query.Where(c =>
            c.VisibilityState.StatusId != 4 && // Hidden
            c.VisibilityState.SetPublicOn <= DateTime.UtcNow &&
            (c.VisibilityState.StatusId == 1 || c.VisibilityState.StatusId == 3) // Visible or Restricted
        );
    }

    private IQueryable<Track> FilterTracksByVisibility(IQueryable<Track> query, int? userId)
    {
        // Фильтрация треков по видимости (Track не наследуется от Collection)
        return query.Where(t =>
            t.VisibilityState.StatusId != 4 && // Hidden
            (t.VisibilityState.StatusId == 1 || t.VisibilityState.StatusId == 3) // Visible or Restricted
        );
    }
}

