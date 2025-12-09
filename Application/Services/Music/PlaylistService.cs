using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Application.DTOs.Music;
using Application.Extensions;
using Application.Response;
using Entities.Models.Library;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Application.Services.Music;

public class PlaylistService(
    IPlaylistRepository repository,
    IImageFileService imageFileService,
    IUserService userService,
    ICollectionService<Album> albumCollectionService,
    ICollectionService<Blend> blendCollectionService,
    ICollectionService<Playlist> playlistCollectionService,
    ILibraryService libraryService,
    IFolderService folderService,
    ITrackService trackService,
    IVisibilityStateService visibilityStateService)
    : CollectionService<Playlist>(repository, libraryService, folderService), IPlaylistService
{
    private const string FavoritePlaylistName = "Favorites";

    public async Task<Playlist> CreatePlaylistAsync(int creatorId, CreatePlaylistDTO dto)
    {
        Playlist playlist = new()
        {
            Name = dto.Name,
            CreatorId = creatorId,
            Cover = dto.Cover != null
                ? await imageFileService.UploadFileAsync(dto.Cover)
                : await imageFileService.GetDefaultAsync(),
            VisibilityState = await visibilityStateService.CreateDefaultAsync()
        };
        playlist = await repository.AddAsync(playlist);

        // Добавляем плейлист в root папку библиотеки пользователя
        User user = await userService.GetByIdValidatedAsync(creatorId);
        Folder rootFolder = await libraryService.GetRootFolderByLibraryIdValidatedAsync(user.LibraryId);
        rootFolder.Collections.Add(playlist);
        await folderService.UpdateAsync(rootFolder);

        return playlist;
    }

    public async Task DeleteAsync(int playlistId, int userId)
    {
        Playlist playlist = await VerifyAccessAsync(playlistId, userId);
        CheckForFavorites(playlist.Name);
        await DeleteAsync(playlistId);
    }

    public async Task<Playlist> UpdateNameAsync(int playlistId, int userId, string newName)
    {
        Playlist playlist = await VerifyAccessAsync(playlistId, userId);
        CheckForFavorites(newName);
        playlist.Name = newName;
        return await repository.UpdateAsync(playlist);
    }

    public async Task UpdatePlaylistCoverAsync(int playlistId, int creatorId, IFormFile newCover)
    {
        Playlist playlist = await VerifyAccessAsync(playlistId, creatorId, true);
        CheckForFavorites(playlist.Name);
        playlist.Cover = await imageFileService.UploadFileAsync(newCover);
        await repository.UpdateAsync(playlist);
    }

    public async Task AddContributorAsync(int playlistId, int contributorId, int creatorId)
    {
        await VerifyAccessAsync(playlistId, creatorId);
        Playlist playlist = await repository.SnInclude(p => p.Contributors).GetByIdValidatedAsync(playlistId);
        CheckForFavorites(playlist.Name);
        HashSet<int> contributorIds = new(playlist.Contributors.Select(c => c.Id));

        if (contributorIds.Contains(contributorId))
            throw ResponseFactory.Create<ConflictResponse>(["User is already a contributor to this playlist."]);
        else if (playlist.Contributors.Any(c => c.Id == creatorId))
            throw ResponseFactory.Create<ConflictResponse>(["User is owner of playlist."]);
        playlist.Contributors.Add(await userService.GetByIdValidatedAsync(contributorId));
        await repository.UpdateAsync(playlist);
    }

    public async Task RemoveContributorAsync(int playlistId, int contributorId, int creatorId)
    {
        await VerifyAccessAsync(playlistId, creatorId);
        Playlist playlist = await repository.SnInclude(p => p.Contributors).GetByIdValidatedAsync(playlistId);
        CheckForFavorites(playlist.Name);
        playlist.Contributors.Remove(await userService.GetByIdValidatedAsync(contributorId));
        await repository.UpdateAsync(playlist);
    }

    public async Task AddTrackToPlaylistAsync(int playlistId, int trackId, int userId)
    {
        Playlist playlist = await VerifyAccessAsync(playlistId, userId, true);
        CheckForFavorites(playlist.Name);
        if (playlist.Tracks.Any(t => t.Id == trackId))
            throw ResponseFactory.Create<ConflictResponse>(["Track is already in the playlist."]);
        playlist.Tracks.Add(await trackService.GetByIdValidatedAsync(trackId));
        await repository.UpdateAsync(playlist);
    }

    public async Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId, int userId)
    {
        Playlist playlist = await VerifyAccessAsync(playlistId, userId, true);
        CheckForFavorites(playlist.Name);
        playlist.Tracks.Remove(await trackService.GetByIdValidatedAsync(trackId));
        await repository.UpdateAsync(playlist);
    }

    public async Task<CursorPageDTO<TrackDTO>> GetPlaylistTracksAsync(int playlistId, string? afterCursor, int limit, int? userId = null)
    {
        int? afterId = null;
        Playlist playlist = await repository
            .SnInclude(p => p.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(playlistId);

        // Validate playlist visibility before returning tracks (ignore if user is creator)
        VisibilityStateValidator.IsAccessible(playlist.VisibilityState, userId, userId.HasValue && playlist.CreatorId == userId.Value ? [userId.Value] : null, "Playlist", playlistId);

        if (!string.IsNullOrEmpty(afterCursor))
        {
            byte[] bytes = Convert.FromBase64String(afterCursor);
            string idString = Encoding.UTF8.GetString(bytes);
            afterId = int.Parse(idString);
        }

        List<Track> tracks = await repository.GetTracksFromPlaylistAfterAsync(playlistId, afterId, limit);

        List<TrackDTO> items = tracks
            .Where(t =>
            {
                if (t.VisibilityState == null)
                    return false;

                IEnumerable<int>? trackAuthorIds = t.TrackArtists?
                    .Where(ta => ta.Artist?.UserId != null)
                    .Select(ta => ta.Artist!.UserId!)
                    .ToList();

                return VisibilityStateValidator.IsAccessible(t.VisibilityState, userId, trackAuthorIds);
            })
            .Select(t => new TrackDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationInSeconds = (int)(t.Duration?.TotalSeconds ?? 0),
                IsExplicit = t.IsExplicit,
                DrivingDisturbingNoises = t.DrivingDisturbingNoises,
                CoverId = t.CoverId,
                AudioFileId = t.LowQualityAudioFileId,
                Artists = t.TrackArtists?.Select(ta => new AuthorDTO
                {
                    Pseudonym = ta.Pseudonym,
                    ArtistId = ta.ArtistId
                }).ToList() ?? new List<AuthorDTO>(),
                Genre = t.Genre != null ? new GenreDTO
                {
                    Id = t.Genre.Id,
                    Name = t.Genre.Name
                } : throw ResponseFactory.Create<BadRequestResponse>(["Track must have a genre"]),
                MoodTags = t.TrackMoodTags?.Select(tmt => new MoodTagDTO
                {
                    Id = tmt.MoodTag.Id,
                    Name = tmt.MoodTag.Name
                }).ToList() ?? new List<MoodTagDTO>()
            }).ToList();

        string? nextCursor = null;
        if (items.Count != limit)
            return new CursorPageDTO<TrackDTO>
            {
                Items = items,
                NextCursor = nextCursor
            };
        string lastId = items.Last().Id.ToString();
        byte[] cursorBytes = Encoding.UTF8.GetBytes(lastId);
        nextCursor = Convert.ToBase64String(cursorBytes);
        return new CursorPageDTO<TrackDTO>
        {
            Items = items,
            NextCursor = nextCursor
        };
    }

    public async Task ImportCollectionToPlaylistAsync<T>(int playlistId, int collectionId, int userId)
        where T : Collection
    {
        Playlist playlist = await VerifyAccessAsync(playlistId, userId, true);
        CheckForFavorites(playlist.Name);

        IEnumerable<Track> tracks = await (typeof(T) switch
        {
            { } t when t == typeof(Album) => albumCollectionService.GetAllTracksAsync(collectionId),
            { } t when t == typeof(Blend) => blendCollectionService.GetAllTracksAsync(collectionId),
            { } t when t == typeof(Playlist) => playlistCollectionService.GetAllTracksAsync(collectionId),
            _ => throw ResponseFactory.Create<BadRequestResponse>(["Collection failed to import"])
        });

        HashSet<int> existingIds = new(playlist.Tracks.Select(t => t.Id));
        foreach (Track track in tracks)
            if (existingIds.Add(track.Id))
                playlist.Tracks.Add(track);
        await repository.UpdateAsync(playlist);
    }

    private void CheckForFavorites(string name)
    {
        if (name == FavoritePlaylistName)
            throw ResponseFactory.Create<BadRequestResponse>([
                "The name 'Favorites' is reserved and cannot be used for playlists."
            ]);
    }

    private async Task<Playlist> VerifyAccessAsync(int playlistId, int userId, bool allowContributor = false)
    {
        Playlist playlist = allowContributor
            ? await repository.SnInclude(p => p.Contributors).SnInclude(p => p.Tracks).GetByIdValidatedAsync(playlistId)
            : await GetByIdValidatedAsync(playlistId);

        bool isCreator = playlist.CreatorId == userId;
        bool isContributor = allowContributor && playlist.Contributors.Any(c => c.Id == userId);
        if (!isCreator && !isContributor)
            throw ResponseFactory.Create<ForbiddenResponse>(["You do not have permission to modify this playlist"]);

        return playlist;
    }

    public async Task UpdateVisibilityStatusAsync(int playlistId, int newVisibilityStatusId, int creatorId)
    {
        Playlist playlist = await VerifyAccessAsync(playlistId, creatorId);
        CheckForFavorites(playlist.Name);
        await base.UpdateVisibilityStatusAsync(playlistId, newVisibilityStatusId);
    }

    public async Task<Playlist> GetByIdWithVisibilityStateAsync(int playlistId)
    {
        return await repository
            .SnInclude(p => p.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .SnInclude(p => p.Creator)
            .SnInclude(p => p.Tracks)
            .SnInclude(p => p.Contributors)
            .GetByIdValidatedAsync(playlistId);
    }
}