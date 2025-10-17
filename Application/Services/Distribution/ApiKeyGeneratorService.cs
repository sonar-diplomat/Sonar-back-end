using System.Security.Cryptography;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class ApiKeyGeneratorService(IDistributorRepository repository) : GenericService<Distributor>(repository), IApiKeyGeneratorService
{
    public async Task<string> GenerateApiKey()
    {
        byte[] key = new byte[24];
        using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(key);
        }
        return await Task.FromResult(Convert.ToBase64String(key));
    }
}