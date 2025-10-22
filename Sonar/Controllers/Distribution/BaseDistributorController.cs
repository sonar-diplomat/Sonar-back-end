using System.IdentityModel.Tokens.Jwt;
using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Sonar.Controllers.Distribution;

public class BaseDistributorController(
    IDistributorAccountService accountService,
    UserManager<User> userManager
) : BaseController(userManager)
{
    [Authorize]
    protected async Task<DistributorAccount> GetDistributorAccountByJwt()
    {
        string? email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                        User.FindFirst("email")?.Value;
        DistributorAccount distributorAccount = await accountService.GetByEmailAsync(email);

        return distributorAccount ?? throw ResponseFactory.Create<UnauthorizedResponse>();
    }
}