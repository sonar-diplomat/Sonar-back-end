using Application.DTOs.Auth;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Repository.Distribution;

public interface IDistributorSessionRepository : IGenericRepository<DistributorSession>
{
    public Task<DistributorSession?> GetByRefreshTokenAsync(string refreshHash);
    public Task<IEnumerable<DistributorSession>> GetAllByDistributorIdAsync(int userId);
    public Task<IEnumerable<ActiveSessionDTO>> GetAllActiveSessionsByDistributorIdAsync(int userId);
}
