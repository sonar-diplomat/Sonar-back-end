using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Repository.UserCore;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int? id);
    Task<IQueryable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task UpdateAvatarImageIdAsync(int userId, int avatarImageId);
    Task RemoveAsync(User user);
    Task RemoveRangeAsync(List<User> users);
    Task<bool> IsUserNameTakenAsync(string userName);
    IQueryable<User> Query();
    Task SaveChangesAsync();
    Task<bool> CheckExists(string publicIdentifier);
    Task<User?> GetByPublicIdentifierAsync(string publicIdentifier);
}