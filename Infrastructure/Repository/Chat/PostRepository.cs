using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    public PostRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}