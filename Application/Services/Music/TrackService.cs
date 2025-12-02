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
    IAlbumService albumService,
    IVisibilityStateService visibilityStateService,
    ILibraryService libraryService,
    IArtistService artistService,
    ITrackArtistService trackArtistService
)
    : GenericService<Track>(repository), ITrackService
{
    public Task<MusicStreamResultDTO?> GetDemoMusicStreamAsync(int songId)
    {
        throw new NotImplementedException();
    }

    public async Task<MusicStreamResultDTO?> GetMusicStreamAsync(int trackId, TimeSpan? startPosition, TimeSpan? length, int preferredPlaybackQualityId)
    {
        Track track = await repository
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);

        // Validate visibility before allowing stream access
        track.VisibilityState.ValidateVisibility("Track", trackId);

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

    public async Task<TrackDTO> GetTrackDtoAsync(int trackId)
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

        // Validate visibility before returning track data
        track.VisibilityState.ValidateVisibility("Track", trackId);

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

    public async Task<Track> CreateTrackAsync(int albumId, UploadTrackDTO dto)
    {
        Album album = await albumService.GetValidatedIncludeTracksAsync(albumId);
        Album albumWithDistributor = await albumService.GetValidatedIncludeDistributorAsync(albumId);
        Album albumWithArtists = await albumService.GetValidatedIncludeAlbumArtistsAsync(albumId);

        Track track = new()
        {
            Title = dto.Title,
            IsExplicit = dto.IsExplicit,
            DrivingDisturbingNoises = dto.DrivingDisturbingNoises,
            VisibilityStateId = (await visibilityStateService.CreateDefaultAsync()).Id,
            LowQualityAudioFileId = (await audioFileService.UploadFileAsync(dto.LowQualityAudioFile)).Id,
            MediumQualityAudioFileId = dto.MediumQualityAudioFile != null
                ? (await audioFileService.UploadFileAsync(dto.MediumQualityAudioFile)).Id
                : null,
            HighQualityAudioFileId = dto.HighQualityAudioFile != null
                ? (await audioFileService.UploadFileAsync(dto.HighQualityAudioFile)).Id
                : null,
            CoverId = albumWithDistributor.CoverId,
            Duration = await audioFileService.GetDurationAsync(dto.LowQualityAudioFile)
        };

        album.Tracks.Add(track);
        track = await repository.AddAsync(track);

        // Добавляем артистов-авторов альбома в авторы трека
        HashSet<int> addedArtistIds = new();
        HashSet<string> addedPseudonyms = new();
        if (albumWithArtists.AlbumArtists != null)
        {
            foreach (AlbumArtist albumArtist in albumWithArtists.AlbumArtists)
            {
                // Пропускаем, если псевдоним уже добавлен
                if (addedPseudonyms.Contains(albumArtist.Pseudonym))
                    continue;

                // Проверяем, не добавлен ли уже этот артист по ID
                if (albumArtist.ArtistId.HasValue && !addedArtistIds.Contains(albumArtist.ArtistId.Value))
                {
                    TrackArtist trackArtist = new()
                    {
                        Pseudonym = albumArtist.Pseudonym,
                        TrackId = track.Id,
                        ArtistId = albumArtist.ArtistId
                    };
                    await trackArtistService.CreateAsync(trackArtist);
                    addedArtistIds.Add(albumArtist.ArtistId.Value);
                    addedPseudonyms.Add(albumArtist.Pseudonym);
                }
                else if (!albumArtist.ArtistId.HasValue)
                {
                    // Если ArtistId не указан, создаем TrackArtist только с Pseudonym
                    TrackArtist trackArtist = new()
                    {
                        Pseudonym = albumArtist.Pseudonym,
                        TrackId = track.Id,
                        ArtistId = null
                    };
                    await trackArtistService.CreateAsync(trackArtist);
                    addedPseudonyms.Add(albumArtist.Pseudonym);
                }
            }
        }

        // Добавляем дополнительных авторов из DTO
        if (dto.AdditionalArtists != null)
        {
            foreach (AuthorDTO authorDto in dto.AdditionalArtists)
            {
                // Пропускаем, если псевдоним уже добавлен
                if (addedPseudonyms.Contains(authorDto.Pseudonym))
                    continue;

                int? artistId = authorDto.ArtistId;
                
                // Если ArtistId указан в DTO, используем его
                if (artistId.HasValue && !addedArtistIds.Contains(artistId.Value))
                {
                    TrackArtist trackArtist = new()
                    {
                        Pseudonym = authorDto.Pseudonym,
                        TrackId = track.Id,
                        ArtistId = artistId
                    };
                    await trackArtistService.CreateAsync(trackArtist);
                    addedArtistIds.Add(artistId.Value);
                    addedPseudonyms.Add(authorDto.Pseudonym);
                }
                // Если ArtistId не указан, создаем TrackArtist только с Pseudonym
                else if (!artistId.HasValue)
                {
                    TrackArtist trackArtist = new()
                    {
                        Pseudonym = authorDto.Pseudonym,
                        TrackId = track.Id,
                        ArtistId = null
                    };
                    await trackArtistService.CreateAsync(trackArtist);
                    addedPseudonyms.Add(authorDto.Pseudonym);
                }
            }
        }

        return track;
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
}