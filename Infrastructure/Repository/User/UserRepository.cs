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
        return await context.Set<User>().Include(u => u.AccessFeatures).Include(u => u.AvatarImage).FirstOrDefaultAsync(u => u.Id == id);
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

    public async Task UpdateAvatarImageIdAsync(int userId, int avatarImageId)
    {
        User? user = await context.Set<User>().FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException($"User with ID {userId} not found.");
        user.AvatarImageId = avatarImageId;
        var entry = context.Entry(user);
        entry.Property(u => u.AvatarImageId).IsModified = true;
        await context.SaveChangesAsync();
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

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public Task<bool> CheckExists(string publicIdentifier)
    {
        return Task.FromResult(context.Set<User>().Any(u => u.PublicIdentifier == publicIdentifier));
    }

    public async Task<User?> GetByPublicIdentifierAsync(string publicIdentifier)
    {
        return await context.Set<User>()
            .Include(u => u.AccessFeatures)
            .Include(u => u.AvatarImage)
            .FirstOrDefaultAsync(u => u.PublicIdentifier == publicIdentifier);
    }
}