using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Repository.UserCore
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int? id);
        Task<IQueryable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task<Task> UpdateAsync(User user);
        Task<Task> RemoveAsync(User user);
        Task<Task> RemoveRangeAsync(List<User> users);
        Task SaveChangesAsync();
    }
}
