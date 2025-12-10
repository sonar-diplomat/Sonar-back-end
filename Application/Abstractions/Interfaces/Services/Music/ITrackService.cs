using Application.DTOs.Music;
using Entities.Models.File;
using Entities.Models.Music;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services;

public interface ITrackService : IGenericService<Track>
{
    Task<MusicStreamResultDTO?> GetMusicStreamAsync(int songId, TimeSpan? startPosition, TimeSpan? length, int preferredPlaybackQualityId, int? userId = null);
    Task<MusicStreamResultDTO?> GetDemoMusicStreamAsync(int songId);
    Task<TrackDTO> GetTrackDtoAsync(int trackId, int? userId = null);
    Task<Track> GetTrackWithVisibilityStateAsync(int trackId);
    Task<AudioFile> UpdateTrackFileAsync(int trackId, int playbackQualityId, IFormFile file);
    Task UpdateVisibilityStatusAsync(int trackId, int newVisibilityStatusId);
    Task<bool> ToggleFavoriteAsync(int trackId, int libraryId);
    Task AssignArtistToTrackAsync(int trackId, AuthorDTO authorDto);
    Task<MusicStreamResultDTO?> GetMusicStreamForDistributorAsync(int trackId, TimeSpan? startPosition, TimeSpan? length, int preferredPlaybackQualityId, int distributorId);
    Task<TrackDTO> GetTrackDtoForDistributorAsync(int trackId, int distributorId);
}