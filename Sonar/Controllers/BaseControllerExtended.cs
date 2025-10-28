using System.IdentityModel.Tokens.Jwt;
using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;

namespace Sonar.Controllers;

public class BaseControllerExtended(
    UserManager<User> userManager,
    IDistributorAccountService accountService,
    IDistributorService distributorService
)
    : BaseController(userManager)
{
    [Authorize]
    protected async Task<DistributorAccount> GetDistributorAccountByJwt()
    {
        string? email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                        User.FindFirst("email")?.Value;
        DistributorAccount distributorAccount = await accountService.GetByEmailAsync(email);

        return distributorAccount ?? throw ResponseFactory.Create<UnauthorizedResponse>();
    }

    [Authorize]
    protected async Task<Distributor> CheckDistributor()
    {
        DistributorAccount distributorAccount = await GetDistributorAccountByJwt();

        if (!Request.Headers.TryGetValue("X-Api-Key", out StringValues apiKey))
            throw ResponseFactory.Create<UnauthorizedResponse>();

        string key = apiKey.ToString();
        if (string.IsNullOrEmpty(key))
            throw ResponseFactory.Create<UnauthorizedResponse>();

        Distributor? distributor = await distributorService.GetByApiKeyAsync(key);

        return !(await accountService.GetAllByDistributor(distributor)).Contains(distributorAccount)
            ? throw ResponseFactory.Create<UnauthorizedResponse>()
            : distributor!;
    }
}