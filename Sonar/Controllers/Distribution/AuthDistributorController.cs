using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        return Ok(new BaseResponse<string>("Session terminated successfully"));
    }

    [HttpPost("register")] // TODO: Admin only
    public async Task<IActionResult> Register(DistributorAccountRegisterDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        return Ok(new BaseResponse<DistributorAccount>(await accountService.RegisterAsync(dto), "Distributor account registered successfully"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        DistributorAccount account = await accountService.GetByEmailValidatedAsync(email);

        using HMACSHA512 hmac = new(account.PasswordSalt);
        byte[]? computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        if (!computedHash.SequenceEqual(account.PasswordHash))
            throw AppExceptionFactory.Create<UnauthorizedException>(["Invalid credentials"]);

        string jwtToken = authService.GenerateJwtToken(account.Email, account.Username);
        string refreshToken = authService.GenerateRefreshToken();
        return Ok(new BaseResponse<(string, string)>((jwtToken, refreshToken), "Distributor account logged in successfully"));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        string refreshHash = authService.ComputeSha256(refreshToken);
        DistributorSession session = await sessionService.GetValidatedByRefreshTokenAsync(refreshHash);
        await sessionService.UpdateLastActiveAsync(session);
        string newAccessToken = authService.GenerateJwtToken(session.DistributorAccount.Email, session.DistributorAccount.Username);
        return Ok(new BaseResponse<(string, string)>((newAccessToken, refreshToken), "Token refreshed successfully"));
    }

    [Authorize]
    [HttpPost("{sessionId:int}/revoke")]
    public async Task<IActionResult> RevokeSessionAsync(int sessionId)
    {
        await CheckAccessFeatures([]);
        DistributorSession session = await sessionService.GetByIdValidatedAsync(sessionId);
        await sessionService.RevokeSessionAsync(session);
        return Ok(new BaseResponse<string>("Session revoked successfully"));
    }


    [Authorize]
    [HttpPost("sessions/revoke-all")]
    public async Task<IActionResult> RevokeAllSessions()
    {
        DistributorAccount account = await GetDistributorAccountByJwt();
        await sessionService.RevokeAllDistributorSessionsAsync(account.Id);
        return Ok(new BaseResponse<string>("All sessions revoked successfully"));
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        User user = await CheckAccessFeatures([]);
        return Ok(new BaseResponse<IEnumerable<ActiveSessionDTO>>(await sessionService.GetAllByUserIdAsync(user.Id), "Sessions retrieved successfully"));
    }
}
