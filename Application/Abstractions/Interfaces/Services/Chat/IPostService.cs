using Application.DTOs;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services;

public interface IPostService : IGenericService<Post>
{
    Task UpdateVisibilityStatusAsync(int trackId, int newVisibilityStatusId);
    Task CreateAsync(int artistId, PostDTO dto);
    Task UpdateAsync(int postId, PostDTO dto);
    Task<IEnumerable<Post>> GetByArtistIdAsync(int artistId);
}