using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Sonar.Controllers;
using System.Security.Claims;

namespace Sonar.Extensions;

public static class BaseControllerExtention
{
    public static async Task<DistributorAccount> GetDistributorAccountByJwtAsync(this BaseController controller)
    {
        var services = controller.HttpContext.RequestServices;

        var userManager = services.GetRequiredService<UserManager<User>>();
        var accountService = services.GetRequiredService<IDistributorAccountService>();

        if (!int.TryParse(
                controller.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                out var id))
        {
            throw ResponseFactory.Create<UnauthorizedResponse>();
        }

        var distributorAccount = await accountService.GetByIdAsync(id);
        return distributorAccount ?? throw ResponseFactory.Create<UnauthorizedResponse>();
    }


    public static async Task<Distributor> CheckDistributorAsync(this BaseController controller)
    {
        var services = controller.HttpContext.RequestServices;

        var accountService = services.GetRequiredService<IDistributorAccountService>();
        var distributorService = services.GetRequiredService<IDistributorService>();

        var distributorAccount = await controller.GetDistributorAccountByJwtAsync() ?? throw ResponseFactory.Create<UnauthorizedResponse>();
        Distributor distributor = (await distributorService.GetByIdAsync(distributorAccount.DistributorId))!;

        return !(await accountService.GetAllByDistributorAsync(distributor.Id)).Contains(distributorAccount)
            ? throw ResponseFactory.Create<UnauthorizedResponse>()
            : distributor!;
    }
}
