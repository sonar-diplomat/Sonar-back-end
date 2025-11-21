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

    /// <summary>
    /// Deletes a distributor account (master account only).
    /// </summary>
    /// <param name="id">The ID of the distributor account to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Distributor account deleted successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Access denied (requires master distributor account).</response>
    /// <response code="404">Distributor account not found.</response>
    [HttpDelete("account/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDistributorAccount(int id)
    {
        await CheckAccessDistributorAccount();
        await accountService.DeleteAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Distributor account deleted successfully"]);
    }

    /// <summary>
    /// Changes the username of a distributor account (master account only).
    /// </summary>
    /// <param name="id">The ID of the distributor account.</param>
    /// <param name="newUserName">The new username.</param>
    /// <returns>Success response upon username change.</returns>
    /// <response code="200">Username changed successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Access denied (requires master distributor account).</response>
    /// <response code="404">Distributor account not found.</response>
    [HttpPut("account/{id:int}/username")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUserName(int id, [FromBody] string newUserName)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangeUserNameAsync(id, newUserName);
        throw ResponseFactory.Create<OkResponse>(["Distributor account username changed successfully"]);
    }

    /// <summary>
    /// Retrieves a specific distributor account by ID (master account only).
    /// </summary>
    /// <param name="id">The ID of the distributor account to retrieve.</param>
    /// <returns>Distributor account DTO with account details.</returns>
    /// <response code="200">Distributor account retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Access denied (requires master distributor account).</response>
    /// <response code="404">Distributor account not found.</response>
    [HttpGet("account/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<DistributorAccountDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Changes the email of a distributor account (master account only).
    /// </summary>
    /// <param name="id">The ID of the distributor account.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <returns>Success response upon email change.</returns>
    /// <response code="200">Email changed successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Access denied (requires master distributor account).</response>
    /// <response code="404">Distributor account not found.</response>
    [HttpPut("account/{id:int}/email")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeEmailDistributorAccount(int id, [FromBody] string newEmail)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangeEmailAsync(id, newEmail);
        throw ResponseFactory.Create<OkResponse>(["Distributor account email changed successfully"]);
    }

    /// <summary>
    /// Changes the password of a distributor account (master account only).
    /// </summary>
    /// <param name="id">The ID of the distributor account.</param>
    /// <param name="dto">Password change DTO containing old and new passwords.</param>
    /// <returns>Success response upon password change.</returns>
    /// <response code="200">Password changed successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Access denied (requires master distributor account).</response>
    /// <response code="404">Distributor account not found.</response>
    /// <response code="417">Old password is incorrect.</response>
    [HttpPut("account/{id:int}/password")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExpectationFailedResponse), StatusCodes.Status417ExpectationFailed)]
    public async Task<IActionResult> ChangePasswordDistributorAccount(int id, [FromBody] DistributorAccountChangePasswordDTO dto)
    {
        await CheckAccessDistributorAccount();
        await accountService.ChangePasswordAsync(id, dto);
        throw ResponseFactory.Create<OkResponse>(["Distributor account password changed successfully"]);
    }
}