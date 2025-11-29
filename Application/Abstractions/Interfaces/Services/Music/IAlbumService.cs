using Application.DTOs.Music;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface IAlbumService : IGenericService<Album>
{
    Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId);
    Task<Album> UpdateNameAsync(int albumId, string newName);
    Task UpdateVisibilityStateAsync(int albumId, int newVisibilityState);
    Task<Album> GetValidatedIncludeTracksAsync(int id);
    Task<Album> GetValidatedIncludeVisibilityStateAsync(int id);
    Task<Album> GetValidatedIncludeDistributorAsync(int id);
    Task<Album> GetValidatedIncludeAlbumArtistsAsync(int id);
    Task UpdateCoverAsync(int albumId, int imageId);
    Task<IEnumerable<Album>> GetAlbumsByDistributorIdAsync(int distributorId);
    Task<IEnumerable<TrackDTO>> GetAlbumTracksAsync(int albumId);
    Task<IEnumerable<Track>> GetAlbumTracksWithVisibilityStateAsync(int albumId);
}