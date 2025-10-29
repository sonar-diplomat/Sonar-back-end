using Application.DTOs;
using Application.DTOs.Music;
using Entities.Models.Music;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services;

public interface IAlbumService : ICollectionService<Album>
{
    Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId);
    Task<Album> UpdateNameAsync(int albumId, string newName);
    Task UpdateVisibilityStatusAsync(int albumId, int newVisibilityState);
    Task<Album> GetValidatedIncludeTracksAsync(int id);
    Task<Album> GetValidatedIncludeVisibilityStateAsync(int id);
    Task UpdateAlbumCoverAsync(int albumId, IFormFile newCover);
}