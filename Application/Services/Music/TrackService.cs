using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Extensions;
using Application.Response;
using Entities.Models.File;
using Entities.Models.Music;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Music;

public class TrackService(
    ITrackRepository repository,
    IAudioFileService audioFileService,
    IVisibilityStateService visibilityStateService,
    ILibraryService libraryService,
    IArtistService artistService,
    ITrackArtistService trackArtistService,
    ITrackAlbumService trackAlbumService
)
    : GenericService<Track>(repository), ITrackService
{
    public Task<MusicStreamResultDTO?> GetDemoMusicStreamAsync(int songId)
    {
        throw new NotImplementedException();
    }

    public async Task<MusicStreamResultDTO?> GetMusicStreamAsync(int trackId, TimeSpan? startPosition, TimeSpan? length, int preferredPlaybackQualityId, int? userId = null)
    {
        Track track = await repository
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);

        // Load TrackArtists with Artist.UserId to check if user is an author
        if (userId.HasValue)
        {
            track = await repository
                .Query()
                .Include(t => t.TrackArtists)
                .ThenInclude(ta => ta.Artist)
                .FirstOrDefaultAsync(t => t.Id == trackId) ?? track;
        }

        // Get author user IDs from TrackArtists
        IEnumerable<int>? authorIds = track.TrackArtists?
            .Where(ta => ta.Artist != null)
            .Select(ta => ta.Artist!.UserId)
            .ToList();

        // Validate visibility before allowing stream access (ignore if user is author)
        VisibilityStateValidator.IsAccessible(track.VisibilityState, userId, authorIds, "Track", trackId);

        // Select audio file based on preferred playback quality with fallback
        int audioFileId = preferredPlaybackQualityId switch
        {
            // High quality (Id = 3)
            3 => track.HighQualityAudioFileId ?? track.MediumQualityAudioFileId ?? track.LowQualityAudioFileId,
            // Medium quality (Id = 2)
            2 => track.MediumQualityAudioFileId ?? track.LowQualityAudioFileId,
            // Low quality (Id = 1) or default
            _ => track.LowQualityAudioFileId
        };

        FileStream? finalStream = await audioFileService.GetMusicStreamAsync(audioFileId, startPosition, length);
        return finalStream != null
            ? new MusicStreamResultDTO(finalStream, "audio/mpeg", true)
            : throw ResponseFactory.Create<UnprocessableContentResponse>(["Unable to process audio stream"]);
    }

    public async Task<TrackDTO> GetTrackDtoAsync(int trackId, int? userId = null)
    {
        Track track = await repository
            .SnInclude(t => t.Cover)
            .SnInclude(t => t.LowQualityAudioFile) // TODO: Get audio file based on user's ClientSettings
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);

        // Загружаем TrackArtists отдельно
        track = await repository
            .Query()
            .Include(t => t.TrackArtists)
            .ThenInclude(ta => ta.Artist)
            .FirstOrDefaultAsync(t => t.Id == trackId) ?? track;

        // Get author user IDs from TrackArtists
        IEnumerable<int>? authorIds = track.TrackArtists?
            .Where(ta => ta.Artist != null)
            .Select(ta => ta.Artist!.UserId)
            .ToList();

        // Validate visibility before returning track data (ignore if user is author)
        VisibilityStateValidator.IsAccessible(track.VisibilityState, userId, authorIds, "Track", trackId);

        return new TrackDTO
        {
            Id = track.Id,
            Title = track.Title,
            DurationInSeconds = (int)(track.Duration?.TotalSeconds ?? 0),
            IsExplicit = track.IsExplicit,
            DrivingDisturbingNoises = track.DrivingDisturbingNoises,
            CoverId = track.CoverId,
            AudioFileId = track.LowQualityAudioFileId,
            Artists = track.TrackArtists?.Select(ta => new AuthorDTO
            {
                Pseudonym = ta.Pseudonym,
                ArtistId = ta.ArtistId
            }).ToList() ?? new List<AuthorDTO>()
        };
    }

    public async Task<Track> GetTrackWithVisibilityStateAsync(int trackId)
    {
        return await repository
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);
    }

    public async Task<AudioFile> UpdateTrackFileAsync(int trackId, int playbackQualityId, IFormFile file)
    {
        Track track = await GetByIdValidatedAsync(trackId);
        AudioFile audioFile = await audioFileService.UploadFileAsync(file);
        Action<int> setAudioFileId = playbackQualityId switch
        {
            1 => id => track.LowQualityAudioFileId = id,
            2 => id => track.MediumQualityAudioFileId = id,
            3 => id => track.HighQualityAudioFileId = id,
            _ => _ => throw ResponseFactory.Create<NotAcceptableResponse>(["Invalid playback quality ID"])
        };
        setAudioFileId(audioFile.Id);
        return audioFile;
    }

    public async Task UpdateVisibilityStatusAsync(int trackId, int newVisibilityStatusId)
    {
        Track track = await repository.SnInclude(a => a.VisibilityState).GetByIdValidatedAsync(trackId);
        track.VisibilityState.StatusId = newVisibilityStatusId;
        await repository.UpdateAsync(track);
    }

    public async Task<bool> ToggleFavoriteAsync(int trackId, int libraryId)
    {
        Playlist playlist = await libraryService.GetFavoritesPlaylistByLibraryIdValidatedAsync(libraryId);
        Track track = await repository.SnInclude(t => t.Collections).GetByIdValidatedAsync(trackId);
        HashSet<int> collectionIds = new(track.Collections.Select(c => c.Id));
        if (collectionIds.Contains(playlist.Id))
        {
            track.Collections.Remove(playlist);
            await repository.UpdateAsync(track);
            return false;
        }

        track.Collections.Add(playlist);
        await repository.UpdateAsync(track);
        return true;
    }

    public async Task AssignArtistToTrackAsync(int trackId, AuthorDTO authorDto)
    {
        Track track = await repository
            .Query()
            .Include(t => t.Collections)
            .FirstOrDefaultAsync(t => t.Id == trackId)
            ?? throw ResponseFactory.Create<NotFoundResponse>([$"{nameof(Track)} not found"]);

        // Проверяем, что трек принадлежит альбому (треки всегда в альбомах)
        Album? album = track.Collections.OfType<Album>().FirstOrDefault();
        if (album == null)
            throw ResponseFactory.Create<BadRequestResponse>(["Track must belong to an album"]);

        // Проверяем, не добавлен ли уже этот артист
        bool alreadyExists = await trackArtistService.GetAllAsync() is IEnumerable<TrackArtist> existing &&
                             existing.Any(ta => ta.TrackId == trackId &&
                                 (authorDto.ArtistId.HasValue && ta.ArtistId == authorDto.ArtistId.Value ||
                                  ta.Pseudonym == authorDto.Pseudonym));

        if (alreadyExists)
            throw ResponseFactory.Create<BadRequestResponse>(["Artist already assigned to this track"]);

        TrackArtist trackArtist = new()
        {
            Pseudonym = authorDto.Pseudonym,
            TrackId = trackId,
            ArtistId = authorDto.ArtistId
        };

        await trackArtistService.CreateAsync(trackArtist);
    }

    public async Task<MusicStreamResultDTO?> GetMusicStreamForDistributorAsync(int trackId, TimeSpan? startPosition, TimeSpan? length, int preferredPlaybackQualityId, int distributorId)
    {
        Track track = await repository
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);

        // Проверяем принадлежность трека дистрибьютору через альбом
        await trackAlbumService.ValidateTrackBelongsToDistributorAsync(trackId, distributorId);

        // Игнорируем visibility state - просто получаем stream
        // Select audio file based on preferred playback quality with fallback
        int audioFileId = preferredPlaybackQualityId switch
        {
            // High quality (Id = 3)
            3 => track.HighQualityAudioFileId ?? track.MediumQualityAudioFileId ?? track.LowQualityAudioFileId,
            // Medium quality (Id = 2)
            2 => track.MediumQualityAudioFileId ?? track.LowQualityAudioFileId,
            // Low quality (Id = 1) or default
            _ => track.LowQualityAudioFileId
        };

        FileStream? finalStream = await audioFileService.GetMusicStreamAsync(audioFileId, startPosition, length);
        return finalStream != null
            ? new MusicStreamResultDTO(finalStream, "audio/mpeg", true)
            : throw ResponseFactory.Create<UnprocessableContentResponse>(["Unable to process audio stream"]);
    }

    public async Task<TrackDTO> GetTrackDtoForDistributorAsync(int trackId, int distributorId)
    {
        Track track = await repository
            .SnInclude(t => t.Cover)
            .SnInclude(t => t.LowQualityAudioFile)
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);

        // Проверяем принадлежность трека дистрибьютору через альбом
        await trackAlbumService.ValidateTrackBelongsToDistributorAsync(trackId, distributorId);

        // Загружаем TrackArtists
        track = await repository
            .Query()
            .Include(t => t.TrackArtists)
            .ThenInclude(ta => ta.Artist)
            .FirstOrDefaultAsync(t => t.Id == trackId) ?? track;

        // Игнорируем visibility state - просто возвращаем DTO
        return new TrackDTO
        {
            Id = track.Id,
            Title = track.Title,
            DurationInSeconds = (int)(track.Duration?.TotalSeconds ?? 0),
            IsExplicit = track.IsExplicit,
            DrivingDisturbingNoises = track.DrivingDisturbingNoises,
            CoverId = track.CoverId,
            AudioFileId = track.LowQualityAudioFileId,
            Artists = track.TrackArtists?.Select(ta => new AuthorDTO
            {
                Pseudonym = ta.Pseudonym,
                ArtistId = ta.ArtistId
            }).ToList() ?? new List<AuthorDTO>()
        };
    }
}