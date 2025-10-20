using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.Music;
using System.Text.RegularExpressions;

namespace Application.Services.Music;

public class TrackService(ITrackRepository repository,IFileService fileService) : GenericService<Track>(repository), ITrackService
{
    public record Range
    {
        public long startBytes;
        public long length;
        public Range(long _startBytes, long _length) {
            startBytes = _startBytes;
            length = _length;
        }
        public Range(long _startBytes) {
            startBytes = _startBytes;    
        }

    }
    public Task<MusicStreamResultDTO?> GetDemoMusicStreamAsync(int songId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<MusicStreamResultDTO?> GetMusicStreamAsync(int trackId, string? rangeHeader) {


        //Track track = await GetByIdValidatedAsync(trackId);
        Range range = new Range(0,0);

        Regex LocalizationLanguage = new Regex(@"(?<=bytes=)(\d+)-(\d+)", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2));

        if(rangeHeader!=null)
        { 
            var match = LocalizationLanguage.Match(rangeHeader);

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
                    range = new Range(startBytes);
            }
        }

        FileStream finalStream = await fileService.GetMusicStreamAsync(0/*track.AudioFileId*/, range.startBytes, range.length);
        return new MusicStreamResultDTO(finalStream, "audio/mpeg", true);
    }
}
