using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.Exception;
using Entities.Models.Distribution;
using Entities.Models.UserCore;

namespace Application.Services.Distribution;

public class LicenseService(ILicenseRepository repository, IUserService userService, IApiKeyGeneratorService keyGeneratorService) : GenericService<License>(repository), ILicenseService
{
    public async Task<License> CreateLicenseAsync(DateTime expirationDate, int issuerId)
    {
        User issuer = await userService.GetByIdValidatedAsync(issuerId);
        if (expirationDate <= DateTime.UtcNow)
            throw AppExceptionFactory.Create<BadRequestException>(["Expiration date must be in the future."]);
        
        License license = new()
        {
            IssuingDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            ExpirationDate = expirationDate,
            LicenseKey = await keyGeneratorService.GenerateApiKey(),
            Issuer = issuer
        };

        return await repository.AddAsync(license);
    }

    public async Task<License?> GetLicenseByKeyAsync(string licenseKey)
    {
        return await repository.GetLicenseByKeyAsync(licenseKey);
    }

    public async Task ExtendLicenseAsync(int id, DateTime extendBy)
    {
        License license = await GetByIdValidatedAsync(id);
        license.ExpirationDate = extendBy;
    }

    public async Task CloseLicenseAsync(int id)
    {
        await repository.RemoveAsync(await GetByIdValidatedAsync(id));
    }
}
