using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Distribution;
using Application.Response;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sonar.Extensions;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class DistributorMasterController(
    UserManager<User> userManager,
    IDistributorSessionService sessionService,
    IDistributorAccountService accountService,
    IDistributorService distributorService,
    AuthService authService)
    : AuthDistributorController(userManager, sessionService, accountService, distributorService, authService)
{
    private readonly IDistributorAccountService accountService = accountService;

    // Helper method - not an API endpoint
    [NonAction]
    [Authorize]
    public async Task CheckAccessDistributorAccount()
    {
        DistributorAccount distributorAccount = await this.GetDistributorAccountByJwtAsync();
        if (!distributorAccount.IsMaster)
            throw ResponseFactory.Create<ForbiddenResponse>(["Access denied. Master distributor account required."]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpDelete("account/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteDistributorAccount(int id)
    {
        await CheckAccessDistributorAccount();
        await accountService.DeleteAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Distributor account deleted successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("account/{id:int}/username")]
    [Authorize]
    public async Task<IActionResult> ChangeUserName(int id, [FromBody] string newUserName)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangeUserNameAsync(id, newUserName);
        throw ResponseFactory.Create<OkResponse>(["Distributor account username changed successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("account/{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetDistributorAccountById(int id)
    {
        await CheckAccessDistributorAccount();
        DistributorAccount? distributorAccount = await accountService.GetByIdAsync(id);
        if (distributorAccount == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Distributor account not found"]);
        DistributorAccountDTO dto = new DistributorAccountDTO
        {
            Id = distributorAccount.Id,
            UserName = distributorAccount.UserName,
            Email = distributorAccount.Email,
            IsMaster = distributorAccount.IsMaster,
            DistributorId = distributorAccount.DistributorId
        };
        throw ResponseFactory.Create<OkResponse<DistributorAccountDTO>>(dto,
            ["Distributor account retrieved successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("account/{id:int}/email")]
    [Authorize]
    public async Task<IActionResult> ChangeEmailDistributorAccount(int id, [FromBody] string newEmail)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangeEmailAsync(id, newEmail);
        throw ResponseFactory.Create<OkResponse>(["Distributor account email changed successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPut("account/{id:int}/password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordDistributorAccount(int id, [FromBody] DistributorAccountChangePasswordDTO dto)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangePasswordAsync(id, dto);
        throw ResponseFactory.Create<OkResponse>(["Distributor account password changed successfully"]);
    }
}