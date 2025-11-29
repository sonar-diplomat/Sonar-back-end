using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Extensions;
using Application.Response;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class DistributorService(IDistributorRepository repository, IArtistService artistService, IArtistRegistrationRequestRepository requestRepository)
    : GenericService<Distributor>(repository), IDistributorService
{
    public override async Task<IEnumerable<Distributor>> GetAllAsync()
    {
        return repository.SnInclude(d => d.License);
    }

    public async Task<Distributor?> GetByApiKeyAsync(string apiKey)
    {
        return await repository.GetByApiKeyAsync(apiKey);
    }

    public async Task<Distributor> CreateDistributorAsync(CreateDistributorDTO dto, int licenseId, int coverId)
    {
        Distributor distributor = new()
        {
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow,
            Description = dto.Description,
            ContactEmail = dto.ContactEmail,
            CoverId = coverId,
            LicenseId = licenseId
        };

        return await repository.AddAsync(distributor);
    }

    public async Task<Distributor> UpdateDistributorAsync(int id, UpdateDistributorDTO dto)
    {
        Distributor distributor = await GetByIdValidatedAsync(id);
        distributor.Name = dto.Name ?? distributor.Name;
        distributor.Description = dto.Description ?? distributor.Description;
        distributor.ContactEmail = dto.ContactEmail ?? distributor.ContactEmail;
        return await repository.UpdateAsync(distributor);
    }

    public async Task<IEnumerable<ArtistRegistrationRequest>> GetAllRegistrationRequestsAsync(int id)
    {
        return (await repository.SnInclude(d => d.ArtistRegistrationRequests)
            .GetByIdValidatedAsync(id)).ArtistRegistrationRequests;
    }

    public async Task<ArtistRegistrationRequest> GetRegistrationRequestByIdAsync(int distributorId, int requestId)
    {
        Distributor distributor = await repository.SnInclude(d => d.ArtistRegistrationRequests)
            .GetByIdValidatedAsync(distributorId);
        ArtistRegistrationRequest? request = distributor.ArtistRegistrationRequests
            .FirstOrDefault(r => r.Id == requestId);
        return request ?? throw ResponseFactory.Create<NotFoundResponse>(["Artist registration request not found"]);
    }

    public async Task<bool> ResolveArtistRequestAsync(int distributorId, int requestId, bool approve)
    {
        ArtistRegistrationRequest request = await GetRegistrationRequestByIdAsync(distributorId, requestId);
        if (request.ResolvedAt != null)
            throw ResponseFactory.Create<BadRequestResponse>(["Artist registration request has already been resolved"]);
        if (approve)
            await artistService.RegisterArtistAsync(request.UserId, request.ArtistName);
        // TODO: Send message to user about the resolution to their request
        request.ResolvedAt = DateTime.UtcNow;
        await requestRepository.UpdateAsync(request);
        return approve;
    }
}