using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Response;
using Entities.Models.Distribution;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Distribution;

public class DistributorAccountService(IDistributorAccountRepository repository, IDistributorService distributorService)
    : GenericService<DistributorAccount>(repository), IDistributorAccountService
{
    public async Task CheckEmailAsync(string email)
    {
        if (await repository.ExistsByEmailAsync(email))
            throw ResponseFactory.Create<ConflictResponse>(["Email is already in use"]);
    }

    public async Task<DistributorAccount?> GetByEmailAsync(string email)
    {
        return await repository.GetByEmailAsync(email);
    }

    public async Task<DistributorAccount> GetByEmailValidatedAsync(string email)
    {
        return await GetByEmailAsync(email) ??
               throw ResponseFactory.Create<NotFoundResponse>(["Distributor account not found"]);
    }

    public async Task<DistributorAccount> RegisterAsync(DistributorAccountRegisterDTO dto)
    {
        await CheckEmailAsync(dto.Email);
        using HMACSHA512 hmac = new();
        DistributorAccount distributor = new()
        {
            Email = dto.Email,
            UserName = dto.UserName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            PasswordSalt = hmac.Key,
            Distributor = await distributorService.GetByIdValidatedAsync(dto.DistributorId)
        };
        return await CreateAsync(distributor);
    }

    public async Task<DistributorAccount> ChangeUserNameAsync(int id, string newUserName)
    {
        DistributorAccount account = await GetByIdValidatedAsync(id);

        account.UserName = newUserName;
        return await UpdateAsync(account);
    }

    public async Task<DistributorAccount> ChangeEmailAsync(int id, string newEmail)
    {
        DistributorAccount account = await GetByIdValidatedAsync(id);
        await CheckEmailAsync(newEmail);
        account.Email = newEmail;
        return await UpdateAsync(account);
    }

    public async Task<DistributorAccount> ChangePasswordAsync(int id, DistributorAccountChangePasswordDTO dto)
    {
        DistributorAccount account = await GetByIdValidatedAsync(id);

        using (HMACSHA512 hmac = new(account.PasswordSalt))
        {
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.OldPassword));
            if (computedHash.Where((t, i) => t != account.PasswordHash[i]).Any())
                throw ResponseFactory.Create<UnauthorizedResponse>(["Old password is incorrect"]);
        }

        using (HMACSHA512 hmac = new())
        {
            account.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.NewPassword));
            account.PasswordSalt = hmac.Key;
        }

        return await UpdateAsync(account);
    }

    public async Task<IEnumerable<DistributorAccount>> GetAllByDistributorAsync(int id)
    {
        return await repository.GetAllByDistributorId(id);
    }

}