using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;

namespace Sonar.Controllers;

public class BaseControllerExtended(
    UserManager<User> userManager,
    IDistributorAccountService accountService,
    IDistributorService distributorService
)
    : BaseController(userManager)
{



    //[Authorize]
    //protected async Task<Distributor> CheckDistributor()
    //{
    //    DistributorAccount distributorAccount = await GetDistributorAccountByJwt();

    //    if (!Request.Headers.TryGetValue("X-Api-Key", out StringValues apiKey))
    //        throw ResponseFactory.Create<UnauthorizedResponse>();

    //    string key = apiKey.ToString();
    //    if (string.IsNullOrEmpty(key))
    //        throw ResponseFactory.Create<UnauthorizedResponse>();

    //    Distributor? distributor = await distributorService.GetByApiKeyAsync(key);

    //    return !(await accountService.GetAllByDistributor(distributor)).Contains(distributorAccount)
    //        ? throw ResponseFactory.Create<UnauthorizedResponse>()
    //        : distributor!;
    //}
}