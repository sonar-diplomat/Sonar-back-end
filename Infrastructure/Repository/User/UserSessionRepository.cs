using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.DTOs.Auth;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.UserCore;

public class UserSessionRepository(SonarContext dbContext) : GenericRepository<UserSession>(dbContext), IUserSessionRepository
{
    public async Task<UserSession?> GetByRefreshToken(string refreshHash)
    {
        return await context.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s =>
                s.RefreshTokenHash == refreshHash &&
                !s.Revoked &&
                s.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<IEnumerable<UserSession>> GetAllByUserIdAsync(int userId)
    {
        return await context.UserSessions
            .Where(s => s.UserId == userId && !s.Revoked)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActiveSessionDTO>> GetAllActiveSessionsByUserIdAsync(int userId)
    {
        return await Task.FromResult(
            context.UserSessions
                .Where(s => s.UserId == userId && !s.Revoked && s.ExpiresAt > DateTime.UtcNow)
                .Select(s => new ActiveSessionDTO
                {
                    Id = s.Id,
                    DeviceName = s.DeviceName,
                    UserAgent = s.UserAgent,
                    IpAddress = s.IPAddress.ToString(),
                    CreatedAt = s.CreatedAt,
                    LastActive = s.LastActive
                }));
    }
}
