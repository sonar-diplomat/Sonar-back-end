using System.Text.RegularExpressions;
using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Application.Response;
using Entities.Models.Music;

namespace Application.Services.Music;

public class TrackService(ITrackRepository repository, IAudioFileService fileService)
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
            await fileService.GetMusicStreamAsync(0 /*track.AudioFileId*/, range.StartBytes, range.Length);
        return finalStream != null
            ? new MusicStreamResultDTO(finalStream, "audio/mpeg", true)
            : throw ResponseFactory.Create<UnprocessableContentResponse>(["Unable to process audio stream"]);
    }

    private record Range(long StartBytes, long Length = 0)
    {
        public long Length { get; } = Length;
        public long StartBytes { get; } = StartBytes;
    }
}