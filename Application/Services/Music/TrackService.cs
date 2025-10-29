using System.Text;
using System.Text.RegularExpressions;
using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Application.DTOs.Music;
using Application.Response;
using Entities.Models.File;
using Entities.Models.Music;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Music;

public class TrackService(
    ITrackRepository repository,
    IAudioFileService audioFileService,
    IAlbumService albumService,
    IVisibilityStateService visibilityStateService)
    : GenericService<Track>(repository), ITrackService
{
    public Task<MusicStreamResultDTO?> GetDemoMusicStreamAsync(int songId)
    {
        throw new NotImplementedException();
    }

    public async Task<MusicStreamResultDTO?> GetMusicStreamAsync(int trackId, string? rangeHeader)
    {
        await GetByIdValidatedAsync(trackId);
        Range range = new(0);

        Regex localizationLanguage = new(@"(?<=bytes=)(\d+)-(\d+)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2));

        if (rangeHeader != null)
        {
            Match match = localizationLanguage.Match(rangeHeader);

            if (match.Success)
            {
                long startBytes = long.Parse(match.Groups[1].Value);
                long? endBytes = null;

                if (match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value))
                    endBytes = long.Parse(match.Groups[2].Value);

                if (endBytes.HasValue)
                {
                    long length = endBytes.Value - startBytes + 1;
                    if (length > 0)
                        range = new Range(startBytes, length);
                }
                else
                {
                    range = new Range(startBytes);
                }
            }
        }

        FileStream? finalStream =
            await audioFileService.GetMusicStreamAsync(0 /*track.AudioFileId*/, range.StartBytes, range.Length);
        return finalStream != null
            ? new MusicStreamResultDTO(finalStream, "audio/mpeg", true)
            : throw ResponseFactory.Create<UnprocessableContentResponse>(["Unable to process audio stream"]);
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

    private record Range(long StartBytes, long Length = 0)
    {
        public long Length { get; } = Length;
        public long StartBytes { get; } = StartBytes;
    }
}