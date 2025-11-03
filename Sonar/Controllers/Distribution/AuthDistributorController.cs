using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Auth;
using Application.Response;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sonar.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class AuthDistributorController(
    UserManager<User> userManager,
    IDistributorSessionService sessionService,
    IDistributorAccountService accountService,
    IDistributorService distributorService,
    AuthService authService
)
    : BaseControllerExtended(userManager, accountService, distributorService)
{
    private readonly IDistributorAccountService accountService = accountService;

    [HttpDelete("session/{id}")]
    public async Task<IActionResult> TerminateSession(int id)
    {
        await sessionService.TerminateSessionAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Session terminated successfully"]);
    }

    [HttpPost("register")]
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

        string jwtToken = authService.GenerateJwtToken(account);
        string refreshToken = authService.GenerateRefreshToken();

        DistributorSession session = new()
        {
            DistributorAccountId = account.Id,
            DeviceName = Request.Headers["X-Device-Name"].ToString() ?? "Unknown device",
            UserAgent = Request.Headers["User-Agent"].ToString() ?? "Unknown",
            IPAddress = HttpContext.Connection.RemoteIpAddress!,
            RefreshTokenHash = authService.ComputeSha256(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            LastActive = DateTime.UtcNow,
            Revoked = false
        };

        // Save refresh token to user
        await sessionService.CreateAsync(session);

        throw ResponseFactory.Create<OkResponse<LoginResponseDTO>>(new LoginResponseDTO(jwtToken, refreshToken, session.Id),
            ["Distributor account logged in successfully"]);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        string refreshHash = authService.ComputeSha256(refreshToken);
        DistributorSession session = await sessionService.GetValidatedByRefreshTokenAsync(refreshHash);
        await sessionService.UpdateLastActiveAsync(session);
        string newAccessToken =
            authService.GenerateJwtToken(session.DistributorAccount);
        throw ResponseFactory.Create<OkResponse<RefreshTokenResponse>>(new RefreshTokenResponse(newAccessToken, refreshToken),
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
        DistributorAccount account = await this.GetDistributorAccountByJwtAsync();
        await sessionService.RevokeAllDistributorSessionsAsync(account.Id);
        throw ResponseFactory.Create<OkResponse>(["All sessions revoked successfully"]);
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        DistributorAccount account = await this.GetDistributorAccountByJwtAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ActiveSessionDTO>>>(
            (await sessionService.GetAllByUserIdAsync(account.Id)).ToList(), ["Sessions retrieved successfully"]);
    }
}