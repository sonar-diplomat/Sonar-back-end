using Application.DTOs.Music;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface IAlbumService : IGenericService<Album>
{
    Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId);
    Task<Album> UpdateNameAsync(int albumId, string newName);
    Task UpdateVisibilityStateAsync(int albumId, int newVisibilityState);
    Task<Album> GetValidatedIncludeTracksAsync(int id);
    Task<Album> GetValidatedIncludeVisibilityStateAsync(int id, int? userId = null);
    Task<Album> GetValidatedIncludeDistributorAsync(int id);
    Task<Album> GetValidatedIncludeAlbumArtistsAsync(int id);
    Task UpdateCoverAsync(int albumId, int imageId);
    Task<IEnumerable<Album>> GetAlbumsByDistributorIdAsync(int distributorId);
    Task<IEnumerable<TrackDTO>> GetAlbumTracksAsync(int albumId, int? userId = null);
    Task<IEnumerable<Track>> GetAlbumTracksWithVisibilityStateAsync(int albumId);
    Task<Track> CreateTrackAsync(int albumId, UploadTrackDTO dto);
    Task<AlbumResponseDTO> GetAlbumDtoForDistributorAsync(int albumId, int distributorId);
    Task<IEnumerable<TrackDTO>> GetAlbumTracksForDistributorAsync(int albumId, int distributorId);
}