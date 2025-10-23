using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Response;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class AuthDistributorController(
    UserManager<User> userManager,
    IDistributorSessionService sessionService,
    IDistributorAccountService accountService,
    AuthService authService
)
    : BaseDistributorController(accountService, userManager)
{
    private readonly IDistributorAccountService accountService = accountService;

    [HttpDelete("session/{id}")]
    public async Task<IActionResult> TerminateSession(int id)
    {
        await sessionService.TerminateSessionAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Session terminated successfully"]);
    }

    [HttpPost("register")] // TODO: Admin only
    public async Task<IActionResult> Register(DistributorAccountRegisterDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        throw ResponseFactory.Create<CreatedResponse<DistributorAccount>>(await accountService.RegisterAsync(dto),
            ["Distributor account registered successfully"]);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        DistributorAccount account = await accountService.GetByEmailValidatedAsync(email);

        using HMACSHA512 hmac = new(account.PasswordSalt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        if (!computedHash.SequenceEqual(account.PasswordHash))
            throw ResponseFactory.Create<UnauthorizedResponse>(["Invalid credentials"]);

        string jwtToken = authService.GenerateJwtToken(account.Email, account.UserName);
        string refreshToken = authService.GenerateRefreshToken();
        throw ResponseFactory.Create<OkResponse<(string, string)>>((jwtToken, refreshToken),
            ["Distributor account logged in successfully"]);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        string refreshHash = authService.ComputeSha256(refreshToken);
        DistributorSession session = await sessionService.GetValidatedByRefreshTokenAsync(refreshHash);
        await sessionService.UpdateLastActiveAsync(session);
        string newAccessToken =
            authService.GenerateJwtToken(session.DistributorAccount.Email, session.DistributorAccount.UserName);
        throw ResponseFactory.Create<OkResponse<(string, string)>>((newAccessToken, refreshToken),
            ["Token refreshed successfully"]);
    }

    [Authorize]
    [HttpPost("{sessionId:int}/revoke")]
    public async Task<IActionResult> RevokeSessionAsync(int sessionId)
    {
        await CheckAccessFeatures([]);
        DistributorSession session = await sessionService.GetByIdValidatedAsync(sessionId);
        await sessionService.RevokeSessionAsync(session);
        throw ResponseFactory.Create<OkResponse>(["Session revoked successfully"]);
    }


    [Authorize]
    [HttpPost("sessions/revoke-all")]
    public async Task<IActionResult> RevokeAllSessions()
    {
        DistributorAccount account = await GetDistributorAccountByJwt();
        await sessionService.RevokeAllDistributorSessionsAsync(account.Id);
        throw ResponseFactory.Create<OkResponse>(["All sessions revoked successfully"]);
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        User user = await CheckAccessFeatures([]);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ActiveSessionDTO>>>(
            await sessionService.GetAllByUserIdAsync(user.Id), ["Sessions retrieved successfully"]);
    }
}