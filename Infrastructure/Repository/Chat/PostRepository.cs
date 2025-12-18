using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Chat;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    private readonly SonarContext dbContext;

    public PostRepository(SonarContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<Post>> GetByArtistIdAsync(int artistId)
    {
        return await dbContext.Posts
            .Include(p => p.Files)
            .Include(p => p.VisibilityState)
            .Where(p => p.ArtistId == artistId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}
