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

    /// <summary>
    /// Terminates a specific distributor session.
    /// </summary>
    /// <param name="id">The session ID to terminate.</param>
    /// <returns>Success response upon termination.</returns>
    /// <response code="200">Session terminated successfully.</response>
    /// <response code="404">Session not found.</response>
    [HttpDelete("session/{id}")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TerminateSession(int id)
    {
        await sessionService.TerminateSessionAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Session terminated successfully"]);
    }

    /// <summary>
    /// Registers a new distributor account.
    /// </summary>
    /// <param name="dto">Distributor account registration data including username, email, and password.</param>
    /// <returns>Created distributor account entity.</returns>
    /// <response code="201">Distributor account registered successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageDistributors' feature).</response>
    /// <response code="400">Invalid registration data or account already exists.</response>
    [HttpPost("register")]
    [Authorize]
    [ProducesResponseType(typeof(CreatedResponse<DistributorAccount>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(DistributorAccountRegisterDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        throw ResponseFactory.Create<CreatedResponse<DistributorAccount>>(await accountService.RegisterAsync(dto),
            ["Distributor account registered successfully"]);
    }

    /// <summary>
    /// Authenticates a distributor account and returns JWT tokens.
    /// </summary>
    /// <param name="email">Distributor account email.</param>
    /// <param name="password">Distributor account password.</param>
    /// <returns>Login response containing JWT access token, refresh token, and session ID.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="401">Invalid credentials.</response>
    /// <remarks>
    /// Requires X-Device-Name header for session tracking.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(OkResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Refreshes an expired distributor access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token string.</param>
    /// <returns>New access token and the same refresh token.</returns>
    /// <response code="200">Token refreshed successfully.</response>
    /// <response code="401">Invalid or expired refresh token.</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(OkResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Revokes a specific distributor session by session ID.
    /// </summary>
    /// <param name="sessionId">The session ID to revoke.</param>
    /// <returns>Success response upon session revocation.</returns>
    /// <response code="200">Session revoked successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Session not found.</response>
    [Authorize]
    [HttpPost("{sessionId:int}/revoke")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeSessionAsync(int sessionId)
    {
        await CheckAccessFeatures([]);
        DistributorSession session = await sessionService.GetByIdValidatedAsync(sessionId);
        await sessionService.RevokeSessionAsync(session);
        throw ResponseFactory.Create<OkResponse>(["Session revoked successfully"]);
    }


    /// <summary>
    /// Revokes all active sessions for the authenticated distributor account.
    /// </summary>
    /// <returns>Success response upon revoking all sessions.</returns>
    /// <response code="200">All sessions revoked successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [Authorize]
    [HttpPost("sessions/revoke-all")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeAllSessions()
    {
        DistributorAccount account = await this.GetDistributorAccountByJwtAsync();
        await sessionService.RevokeAllDistributorSessionsAsync(account.Id);
        throw ResponseFactory.Create<OkResponse>(["All sessions revoked successfully"]);
    }

    /// <summary>
    /// Retrieves all active sessions for the authenticated distributor account.
    /// </summary>
    /// <returns>List of active sessions with device information and last active times.</returns>
    /// <response code="200">Sessions retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [Authorize]
    [HttpGet("sessions")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ActiveSessionDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSessions()
    {
        DistributorAccount account = await this.GetDistributorAccountByJwtAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ActiveSessionDTO>>>(
            (await sessionService.GetAllByUserIdAsync(account.Id)).ToList(), ["Sessions retrieved successfully"]);
    }
}