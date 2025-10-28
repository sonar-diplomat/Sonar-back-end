using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.UserCore;

public class UserRepository(SonarContext context) : IUserRepository
{
    public IQueryable<User> Query()
    {
        return context.Set<User>().AsQueryable();
    }

    public async Task<User?> GetByIdAsync(int? id)
    {
        return await context.Set<User>().Include(u => u.AccessFeatures).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IQueryable<User>> GetAllAsync()
    {
        return await Task.FromResult(context.Set<User>().AsQueryable());
    }

    public async Task<User> AddAsync(User user)
    {
        await context.Set<User>().AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        context.Set<User>().Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task RemoveAsync(User user)
    {
        context.Set<User>().Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<User> users)
    {
        context.Set<User>().RemoveRange(users);
        await context.SaveChangesAsync();
    }

    public Task<bool> IsUserNameTakenAsync(string UserName)
    {
        return Task.FromResult(context.Set<User>().Any(u => u.UserName == UserName));
    }
}
