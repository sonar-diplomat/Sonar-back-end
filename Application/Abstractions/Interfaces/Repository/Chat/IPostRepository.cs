using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Repository.Chat;

public interface IPostRepository : IGenericRepository<Post>
{
    Task<IEnumerable<Post>> GetByArtistIdAsync(int artistId);
}
