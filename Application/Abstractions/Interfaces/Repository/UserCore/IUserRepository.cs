using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Repository.UserCore;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int? id);
    Task<IQueryable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task RemoveAsync(User user);
    Task RemoveRangeAsync(List<User> users);
}
