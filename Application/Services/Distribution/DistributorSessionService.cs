using System.Net;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class DistributorSessionService(IDistributorSessionRepository repository, IDistributorService distributorService)
    : GenericService<DistributorSession>(repository), IDistributorSessionService
{
    public async Task<IEnumerable<DistributorSession>?> GetByDistributorIdAsync(int id)
    {
        Distributor distributor = await distributorService.GetByIdValidatedAsync(id);
        return distributor.Sessions;
    }

    public async Task<DistributorSession?> CreateSessionAsync(SessionDTO dto)
    {
        if (dto.IpAddress == null || dto.UserAgent == null || dto.DeviceName == null)
            return null;
        
        DistributorSession session = new()
        { 
            IPAddress = IPAddress.Parse(dto.IpAddress),
            UserAgent = dto.UserAgent,
            DeviceName = dto.DeviceName,
            LastActive = DateTime.UtcNow,
            Distributor = await distributorService.GetByIdValidatedAsync(dto.DistributorId)
        };
        
        return await repository.AddAsync(session);
    }

    public async Task<DistributorSession> UpdateSessionAsync(int id, SessionDTO dto)
    {
        DistributorSession session = await GetByIdValidatedAsync(id);
        session.IPAddress = dto.IpAddress != null ? IPAddress.Parse(dto.IpAddress) : session.IPAddress;
        session.UserAgent = dto.UserAgent ?? session.UserAgent;
        session.DeviceName = dto.DeviceName ?? session.DeviceName;
        session.LastActive = DateTime.UtcNow;
        return await repository.UpdateAsync(session);
    }

    public async Task TerminateSessionAsync(int id)
    {
        DistributorSession session = await GetByIdValidatedAsync(id);
        await repository.RemoveAsync(session);
    }
}
