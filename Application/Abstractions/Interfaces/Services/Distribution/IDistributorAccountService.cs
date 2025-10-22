using Application.DTOs;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services;

public interface IDistributorAccountService : IGenericService<DistributorAccount>
{
    Task CheckEmailAsync(string email);
    Task<DistributorAccount?> GetByEmailAsync(string email);
    Task<DistributorAccount> GetByEmailValidatedAsync(string email);
    Task<DistributorAccount> RegisterAsync(DistributorAccountRegisterDTO dto);
    Task<DistributorAccount> ChangeUserNameAsync(int id, string newUserName);
    Task<DistributorAccount> ChangeEmailAsync(int id, string newEmail);
    Task<DistributorAccount> ChangePasswordAsync(int id, DistributorAccountChangePasswordDTO dto);
}