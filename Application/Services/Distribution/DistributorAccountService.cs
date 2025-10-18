using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class DistributorAccountService(IDistributorAccountRepository repository, IDistributorService distributorService)
    : GenericService<DistributorAccount>(repository), IDistributorAccountService
{
    public async Task CheckEmailAsync(string email)
    {
        if (!await repository.ExistsByEmailAsync(email))
            throw ResponseFactory.Create<ConflictResponse>(["Email is already in use"]);
    }

    public async Task<DistributorAccount?> GetByEmailAsync(string email)
    {
        return await repository.GetByEmailAsync(email);
    }

    public async Task<DistributorAccount> GetByEmailValidatedAsync(string email)
    {
        return await GetByEmailAsync(email) ?? throw ResponseFactory.Create<NotFoundResponse>(["Distributor account not found"]);
    }

    public async Task<DistributorAccount> RegisterAsync(DistributorAccountRegisterDTO dto)
    {
        await CheckEmailAsync(dto.Email);
        using HMACSHA512 hmac = new();
        DistributorAccount distributor = new()
        {
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            PasswordSalt = hmac.Key,
            Distributor = await distributorService.GetByIdValidatedAsync(dto.DistributorId)
        };
        return await CreateAsync(distributor);
    }
}
