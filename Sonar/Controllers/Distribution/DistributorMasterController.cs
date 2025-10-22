using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Distribution;

public class DistributorMasterController(
    UserManager<User> userManager,
    IDistributorSessionService sessionService,
    IDistributorAccountService accountService,
    AuthService authService)
    : AuthDistributorController(userManager, sessionService, accountService, authService)
{
    IDistributorAccountService accountService = accountService;

    [Authorize]
    public async Task CheckAccessDistributorAccount()
    {
        DistributorAccount distributorAccount = await GetDistributorAccountByJwt();
        if (!distributorAccount.IsMaster)
            throw ResponseFactory.Create<ForbiddenResponse>(["Access denied. Master distributor account required."]);
    }

    public async Task<IActionResult> DeleteDistributorAccount(int id)
    {
        await CheckAccessDistributorAccount();
        throw ResponseFactory.Create<OkResponse>(["Distributor account deleted successfully"]);
    }

    public async Task<IActionResult> ChangeUserName(int id, string newUserName)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangeUserNameAsync(id, newUserName);
        throw ResponseFactory.Create<OkResponse>(["Distributor account username changed successfully"]);
    }

    public async Task<IActionResult> GetDistributorAccountById(int id)
    {
        await CheckAccessDistributorAccount();
        DistributorAccount? distributorAccount = await accountService.GetByIdAsync(id);
        if (distributorAccount == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Distributor account not found"]);
        throw ResponseFactory.Create<OkResponse<DistributorAccount>>(distributorAccount,
            ["Distributor account retrieved successfully"]);
    }

    public async Task<IActionResult> ChangeEmailDistributorAccount(int id, string newEmail)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangeEmailAsync(id, newEmail);
        throw ResponseFactory.Create<OkResponse>(["Distributor account email changed successfully"]);
    }

    public async Task<IActionResult> ChangePasswordDistributorAccount(int id, DistributorAccountChangePasswordDTO dto)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangePasswordAsync(id, dto);
        throw ResponseFactory.Create<OkResponse>(["Distributor account password changed successfully"]);
    }
}