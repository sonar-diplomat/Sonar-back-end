using Application.DTOs;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services;

public interface IDistributorService : IGenericService<Distributor>
{
    Task<Distributor?> GetByApiKeyAsync(string apiKey);
    Task<Distributor> CreateDistributorAsync(CreateDistributorDTO dto, int licenseId, int coverId);
    Task<Distributor> UpdateDistributorAsync(int id, UpdateDistributorDTO dto);
    Task<IEnumerable<ArtistRegistrationRequest>> GetAllRegistrationRequestsAsync(int id);
    Task<ArtistRegistrationRequest> GetRegistrationRequestByIdAsync(int distributorId, int requestId);
    Task<bool> ResolveArtistRequestAsync(int distributorId, int requestId, bool approve);
}