using Application.DTOs;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services;

public interface IDistributorAccountService : IGenericService<DistributorAccount>
{
    Task CheckEmailAsync(string login);
    Task<DistributorAccount?> GetByEmailAsync(string email);
    Task<DistributorAccount> GetByEmailValidatedAsync(string email);
    Task<DistributorAccount> RegisterAsync(DistributorAccountRegisterDTO dto);
}
