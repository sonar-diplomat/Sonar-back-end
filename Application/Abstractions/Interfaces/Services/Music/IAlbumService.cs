using Application.DTOs;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface IAlbumService : IGenericService<Album>
{
    Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId);
    Task<Album> UpdateNameAsync(int albumId, string newName);
    Task UpdateVisibilityStateAsync(int albumId, int newVisibilityState);
    Task<Album> GetValidatedIncludeTracksAsync(int id);
    Task<Album> GetValidatedIncludeVisibilityStateAsync(int id);
}