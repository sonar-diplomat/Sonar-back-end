using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class DistributorService(IDistributorRepository repository)
    : GenericService<Distributor>(repository), IDistributorService
{
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
}
