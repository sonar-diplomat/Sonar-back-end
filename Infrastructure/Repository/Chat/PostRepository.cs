using Application.Abstractions.Interfaces.Repository.Chat;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Chat
{
    public class PostRepository : GenericRepository<Entities.Models.Post>, IPostRepository
    {
        public PostRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
