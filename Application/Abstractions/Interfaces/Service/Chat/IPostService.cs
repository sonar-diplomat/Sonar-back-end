using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IPostService
    {
        Task<Post> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post> CreateAsync(Post post);
        Task<Post> UpdateAsync(Post post);
        Task<bool> DeleteAsync(int id);
    }
}

