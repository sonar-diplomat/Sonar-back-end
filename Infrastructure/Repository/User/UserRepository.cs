using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserCore;

public class UserRepository : IUserRepository
{
    private readonly SonarContext _context;

    public UserRepository(SonarContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int? id)
    {
        return await _context.Set<User>().FindAsync(id);
    }
    public async Task<IQueryable<User>> GetAllAsync()
    {
        return await Task.FromResult(_context.Set<User>().AsQueryable());
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Set<User>().AddAsync(user);
        return user;
    }
    public async Task<Task> UpdateAsync(User user)
    {
        _context.Set<User>().Update(user);
        return Task.CompletedTask;
    }

    public async Task<Task> RemoveAsync(User user)
    {
        _context.Set<User>().Remove(user);
        return Task.CompletedTask;
    }

    public async Task<Task> RemoveRangeAsync(List<User> users)
    {
        _context.Set<User>().RemoveRange(users);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

