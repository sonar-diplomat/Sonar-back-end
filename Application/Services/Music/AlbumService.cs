using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Extensions;
using Application.Response;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Music;

public class AlbumService(
    IAlbumRepository repository,
    ITrackRepository trackRepository,
    IVisibilityStateService visibilityStateService,
    IImageFileService imageFileService,
    IAudioFileService audioFileService,
    IAlbumArtistService albumArtistService,
    ITrackArtistService trackArtistService,
    IArtistService artistService,
    ILibraryService libraryService,
    IFolderService folderService,
    ITrackService trackService,
    IMoodTagRepository moodTagRepository,
    IAlbumMoodTagRepository albumMoodTagRepository,
    ITrackMoodTagRepository trackMoodTagRepository
) : CollectionService<Album>(repository, libraryService, folderService), IAlbumService
{
    public async Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId)
    {
        Album album = new()
        {
            Name = dto.Name,
            Cover = await imageFileService.UploadFileAsync(dto.Cover),
            VisibilityStateId = (await visibilityStateService.CreateDefaultAsync()).Id,
            DistributorId = distributorId,
            GenreId = dto.GenreId
        };

        album = await repository.AddAsync(album);

        foreach (string authorPseudonym in dto.Authors)
        {
            AlbumArtist albumArtist = new()
            {
                Pseudonym = authorPseudonym,
                AlbumId = album.Id,
                ArtistId = (await artistService.GetByNameAsync(authorPseudonym))?.Id
            };
            await albumArtistService.CreateAsync(albumArtist);
        }

        if (dto.MoodTagIds != null && dto.MoodTagIds.Any())
        {
            if (dto.MoodTagIds.Count() > 3)
            {
                throw ResponseFactory.Create<BadRequestResponse>(["Mood tags cannot exceed 3"]);
            }

            var allMoodTags = await moodTagRepository.GetAllAsync();
            var validMoodTagIds = allMoodTags
                .Where(mt => dto.MoodTagIds.Contains(mt.Id))
                .Select(mt => mt.Id)
                .ToList();

            if (validMoodTagIds.Count != dto.MoodTagIds.Count())
            {
                throw ResponseFactory.Create<BadRequestResponse>(["One or more mood tags are invalid"]);
            }

            foreach (int moodTagId in validMoodTagIds)
            {
                AlbumMoodTag albumMoodTag = new()
                {
                    AlbumId = album.Id,
                    MoodTagId = moodTagId
                };
                await albumMoodTagRepository.AddAsync(albumMoodTag);
            }
        }

        return album;
    }

    public async Task<Album> UpdateNameAsync(int albumId, string newName)
    {
        Album album = await GetByIdValidatedAsync(albumId);
        album.Name = newName;
        return await repository.UpdateAsync(album);
    }

    public async Task UpdateVisibilityStateAsync(int albumId, int newVisibilityState)
    {
        Album album = await repository.SnInclude(a => a.VisibilityState).GetByIdValidatedAsync(albumId);
        album.VisibilityStateId = newVisibilityState;
        await repository.UpdateAsync(album);
    }

    public async Task<Album> GetValidatedIncludeTracksAsync(int id)
    {
        return await repository.SnInclude(a => a.Tracks).GetByIdValidatedAsync(id);
    }

    public async Task<Album> GetValidatedIncludeVisibilityStateAsync(int id, int? userId = null)
    {
        Album album = await repository
            .SnInclude(a => a.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(id);

        album = await repository
            .Query()
            .Include(a => a.AlbumArtists)
            .ThenInclude(aa => aa.Artist)
            .Include(a => a.Genre)
            .Include(a => a.AlbumMoodTags)
            .ThenInclude(amt => amt.MoodTag)
            .FirstOrDefaultAsync(a => a.Id == id) ?? album;

        return album;
    }

    public async Task<Album> GetValidatedIncludeDistributorAsync(int id)
    {
        return await repository.SnInclude(a => a.Distributor).GetByIdValidatedAsync(id);
    }

    public async Task<Album> GetValidatedIncludeAlbumArtistsAsync(int id)
    {
        return await repository
            .Query()
            .Include(a => a.AlbumArtists)
            .ThenInclude(aa => aa.Artist)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw ResponseFactory.Create<NotFoundResponse>([$"{nameof(Album)} not found"]);
    }

    public async Task UpdateCoverAsync(int albumId, int imageId)
    {
        Album album = await repository.SnInclude(a => a.Tracks).GetByIdValidatedAsync(albumId);

        album.CoverId = imageId;
        foreach (var track in album.Tracks) { track.CoverId = imageId; }

        await repository.UpdateAsync(album);
    }

    public async Task<IEnumerable<Album>> GetAlbumsByDistributorIdAsync(int distributorId)
    {
        return await repository.GetByDistributorIdAsync(distributorId);
    }

    public async Task<IEnumerable<TrackDTO>> GetAlbumTracksAsync(int albumId, int? userId = null)
    {
        await GetByIdValidatedAsync(albumId);

        List<Track> tracks = await repository.GetTracksFromAlbumAsync(albumId);
        
        tracks = await trackRepository
            .Query()
            .Include(t => t.TrackArtists)
            .ThenInclude(ta => ta.Artist)
            .Include(t => t.Genre)
            .Include(t => t.TrackMoodTags)
            .ThenInclude(tmt => tmt.MoodTag)
            .Include(t => t.VisibilityState)
            .ThenInclude(vs => vs.Status)
            .Where(t => tracks.Select(tr => tr.Id).Contains(t.Id))
            .ToListAsync();

        List<TrackDTO> trackDTOs = tracks
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
            })
            .ToList();

        return trackDTOs;
    }

    public async Task<IEnumerable<Track>> GetAlbumTracksWithVisibilityStateAsync(int albumId)
    {
        await GetByIdValidatedAsync(albumId);
        return await repository.GetTracksFromAlbumAsync(albumId);
    }

    /// <summary>
    /// Удаляет альбом и все его треки каскадно.
    /// </summary>
    public override async Task DeleteAsync(int id)
    {
        // Проверяем существование альбома
        await GetByIdValidatedAsync(id);

        // Удаляем все треки альбома перед удалением альбома
        var tracks = await repository.GetTracksFromAlbumAsync(id);
        foreach (var track in tracks)
        {
            await trackRepository.RemoveAsync(track);
        }

        // Удаляем сам альбом
        await base.DeleteAsync(id);
    }

    public async Task<Track> CreateTrackAsync(int albumId, UploadTrackDTO dto)
    {
        Album album = await GetValidatedIncludeTracksAsync(albumId);
        Album albumWithDistributor = await GetValidatedIncludeDistributorAsync(albumId);
        Album albumWithArtists = await GetValidatedIncludeAlbumArtistsAsync(albumId);
        
        Album albumWithGenreAndMoodTags = await repository
            .Query()
            .Include(a => a.Genre)
            .Include(a => a.AlbumMoodTags)
            .ThenInclude(amt => amt.MoodTag)
            .FirstOrDefaultAsync(a => a.Id == albumId)
            ?? album;

        int genreId = dto.GenreId != 0 ? dto.GenreId : 
                     (albumWithGenreAndMoodTags.GenreId ?? throw ResponseFactory.Create<BadRequestResponse>(["Genre is required for track. Either specify GenreId in request or set GenreId on album"]));

        IEnumerable<int>? moodTagIds = dto.MoodTagIds != null && dto.MoodTagIds.Any() 
            ? dto.MoodTagIds 
            : (albumWithGenreAndMoodTags.AlbumMoodTags?.Select(amt => amt.MoodTagId).ToList());

        if (moodTagIds != null && moodTagIds.Count() > 3)
        {
            throw ResponseFactory.Create<BadRequestResponse>(["Mood tags cannot exceed 3"]);
        }

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
            GenreId = genreId,
            Duration = await audioFileService.GetDurationAsync(dto.LowQualityAudioFile)
        };

        album.Tracks.Add(track);
        track = await trackRepository.AddAsync(track);

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

        if (moodTagIds != null && moodTagIds.Any())
        {
            var allMoodTags = await moodTagRepository.GetAllAsync();
            var validMoodTagIds = allMoodTags
                .Where(mt => moodTagIds.Contains(mt.Id))
                .Select(mt => mt.Id)
                .ToList();

            if (validMoodTagIds.Count != moodTagIds.Count())
            {
                throw ResponseFactory.Create<BadRequestResponse>(["One or more mood tags are invalid"]);
            }

            foreach (int moodTagId in validMoodTagIds)
            {
                TrackMoodTag trackMoodTag = new()
                {
                    TrackId = track.Id,
                    MoodTagId = moodTagId
                };
                await trackMoodTagRepository.AddAsync(trackMoodTag);
            }
        }

        return track;
    }

    public async Task<AlbumResponseDTO> GetAlbumDtoForDistributorAsync(int albumId, int distributorId)
    {
        Album album = await repository
            .SnInclude(a => a.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(albumId);

        // Проверяем принадлежность альбома дистрибьютору
        Album albumWithDistributor = await GetValidatedIncludeDistributorAsync(albumId);
        if (albumWithDistributor.DistributorId != distributorId)
            throw ResponseFactory.Create<UnauthorizedResponse>(["Album does not belong to your distributor"]);

        // Загружаем AlbumArtists
        Album albumWithArtists = await GetValidatedIncludeAlbumArtistsAsync(albumId);

        // Игнорируем visibility state - просто возвращаем DTO
        return new AlbumResponseDTO
        {
            Id = album.Id,
            Name = album.Name,
            CoverId = album.CoverId,
            DistributorName = albumWithDistributor.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0,
            Authors = albumWithArtists.AlbumArtists?.Select(aa => new AuthorDTO
            {
                Pseudonym = aa.Pseudonym,
                ArtistId = aa.ArtistId
            }).ToList() ?? new List<AuthorDTO>()
        };
    }

    public async Task<IEnumerable<TrackDTO>> GetAlbumTracksForDistributorAsync(int albumId, int distributorId)
    {
        // Проверяем принадлежность альбома дистрибьютору
        Album albumWithDistributor = await GetValidatedIncludeDistributorAsync(albumId);
        if (albumWithDistributor.DistributorId != distributorId)
            throw ResponseFactory.Create<UnauthorizedResponse>(["Album does not belong to your distributor"]);

        // Получаем треки альбома без фильтрации по visibility
        List<Track> tracks = await repository.GetTracksFromAlbumAsync(albumId);

        // Конвертируем в DTOs без проверки visibility state
        List<TrackDTO> trackDTOs = tracks
            .Select(t => new TrackDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationInSeconds = (int)(t.Duration?.TotalSeconds ?? 0),
                IsExplicit = t.IsExplicit,
                DrivingDisturbingNoises = t.DrivingDisturbingNoises,
                CoverId = t.CoverId,
                AudioFileId = t.LowQualityAudioFileId,
                Genre = t.Genre != null
                    ? new GenreDTO { Id = t.Genre.Id, Name = t.Genre.Name }
                    : throw ResponseFactory.Create<BadRequestResponse>(["Track must have a genre"]),
                Artists = t.TrackArtists?.Select(ta => new AuthorDTO
                {
                    Pseudonym = ta.Pseudonym,
                    ArtistId = ta.ArtistId
                }).ToList() ?? new List<AuthorDTO>(),
                MoodTags = t.TrackMoodTags?
                    .Select(tmt => new MoodTagDTO { Id = tmt.MoodTag.Id, Name = tmt.MoodTag.Name })
                    .ToList() ?? new List<MoodTagDTO>()
            })
            .ToList();

        return trackDTOs;
    }
}