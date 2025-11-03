using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.DTOs.Auth;
using Application.Extensions;
using Entities.Models.Distribution;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Distribution;

public class DistributorSessionRepository(SonarContext dbContext) : GenericRepository<DistributorSession>(dbContext), IDistributorSessionRepository
{
    public async Task<DistributorSession?> GetByRefreshTokenAsync(string refreshHash)
    {
        return await RepositoryIncludeExtensions.Include(context.DistributorSessions, s => s.DistributorAccount)
            .FirstOrDefaultAsync(s =>
                s.RefreshTokenHash == refreshHash &&
                !s.Revoked &&
                s.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<IEnumerable<DistributorSession>> GetAllByDistributorIdAsync(int userId)
    {
        return await context.DistributorSessions
            .Where(s => s.DistributorAccountId == userId && !s.Revoked)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActiveSessionDTO>> GetAllActiveSessionsByDistributorIdAsync(int distributorId)
    {
        return await Task.FromResult(
            context.DistributorSessions
                .Where(s => s.DistributorAccountId == distributorId && !s.Revoked && s.ExpiresAt > DateTime.UtcNow)
                .Select(s => new ActiveSessionDTO
                {
                    Id = s.Id,
                    DeviceName = s.DeviceName,
                    UserAgent = s.UserAgent,
                    IpAddress = s.IPAddress == null ? null : s.IPAddress.ToString(),
                    CreatedAt = s.CreatedAt,
                    LastActive = s.LastActive
                }));
    }
}
