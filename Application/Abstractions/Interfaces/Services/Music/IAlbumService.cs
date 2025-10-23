using Application.DTOs;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface IAlbumService : IGenericService<Album>
{
    Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId);
    Task<Album> UpdateNameAsync(int albumId, string newName);
}