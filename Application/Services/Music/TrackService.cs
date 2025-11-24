using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Extensions;
using Application.Response;
using Entities.Models.File;
using Entities.Models.Music;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Music;

public class TrackService(
    ITrackRepository repository,
    IAudioFileService audioFileService,
    IAlbumService albumService,
    IVisibilityStateService visibilityStateService,
    ILibraryService libraryService
)
    : GenericService<Track>(repository), ITrackService
{
    public Task<MusicStreamResultDTO?> GetDemoMusicStreamAsync(int songId)
    {
        throw new NotImplementedException();
    }

    public async Task<MusicStreamResultDTO?> GetMusicStreamAsync(int trackId, TimeSpan? startPosition, TimeSpan? length)
    {
        Track track = await repository
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);
        
        // Validate visibility before allowing stream access
        track.VisibilityState.ValidateVisibility("Track", trackId);
        
        // TODO: Use settings to determine track quality (LowQualityAudioFileId, MediumQualityAudioFileId, HighQualityAudioFileId)
        int audioFileId = track.LowQualityAudioFileId;
        
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
            .SnInclude(t => t.Artists)
            .SnInclude(t => t.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(trackId);

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
            Artists = track.Artists.Select(a => a.ArtistName)
        };
    }

    public async Task<Track> CreateTrackAsync(int albumId, UploadTrackDTO dto)
    {
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
            CoverId = (await albumService.GetByIdValidatedAsync(albumId)).CoverId,
            Duration = await audioFileService.GetDurationAsync(dto.LowQualityAudioFile)
        };
        (await albumService.GetValidatedIncludeTracksAsync(albumId)).Tracks.Add(track);
        return await repository.AddAsync(track);
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
}