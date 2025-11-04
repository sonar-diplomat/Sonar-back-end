using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs.Auth;
using Application.Response;
using Entities.Enums;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IConfiguration configuration,
    IUserService userService,
    IEmailSenderService emailSenderService,
    IUserSessionService userSessionService,
    AuthService authService
)
    : BaseController(userManager)
{
    private readonly string frontEndUrl = configuration["FrontEnd-Url"]!;
    private readonly UserManager<User> userManager = userManager;

    /// <summary>
    /// Registers a new user account in the system.
    /// </summary>
    /// <param name="model">User registration data including username, email, password, and personal information.</param>
    /// <returns>Success response upon successful registration.</returns>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Invalid registration data or user already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(UserRegisterDTO model)
    {
        User user = await userService.CreateUserShellAsync(model);
        IdentityResult result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
            throw ResponseFactory.Create<OkResponse>(["Registration successfull"]);
        throw ResponseFactory.Create<BadRequestResponse>(result.Errors.Select(e => e.Description.ToString()).ToArray()!);
    }

    /// <summary>
    /// Authenticates a user and returns JWT access and refresh tokens.
    /// </summary>
    /// <param name="userIdentifier">Username or email address.</param>
    /// <param name="password">User password.</param>
    /// <returns>Login response containing access token, refresh token, and session ID. If 2FA is enabled, returns confirmation message.</returns>
    /// <response code="200">Login successful or 2FA code sent.</response>
    /// <response code="400">User not found.</response>
    /// <response code="417">Invalid credentials.</response>
    /// <remarks>
    /// Requires X-Device-Name header for session tracking.
    /// If 2FA is enabled, a verification code will be sent to the user's email.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(OkResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExpectationFailedResponse), StatusCodes.Status417ExpectationFailed)]
    public async Task<IActionResult> Login(string userIdentifier, string password)
    {
        User? user = await userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == userIdentifier || u.Email == userIdentifier);

        if (user == null) ResponseFactory.Create<BadRequestResponse>([$"User {userIdentifier} not found"]);

        SignInResult result = await signInManager.CheckPasswordSignInAsync(
            user!, password, false);

        if (!result.Succeeded) ResponseFactory.Create<ExpectationFailedResponse>();

        if (user!.Enabled2FA)
        {
            string code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
            await emailSenderService.SendEmailAsync(
                user.Email!,
                MailGunTemplates.twoFA,
                new Dictionary<string, string>
                {
                    { "code", code }
                });

            throw ResponseFactory.Create<OkResponse>(["2FA code sent to email"]);
        }

        // Generate both tokens
        string accessToken = authService.GenerateJwtToken(user, Request.Headers["X-Device-Name"].ToString());
        string refreshToken = authService.GenerateRefreshToken();

        UserSession session = new()
        {
            UserId = user.Id,
            DeviceName = Request.Headers["X-Device-Name"].ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IPAddress = HttpContext.Connection.RemoteIpAddress!,
            RefreshTokenHash = authService.ComputeSha256(refreshToken),
            //ExpiresAt = DateTime.UtcNow.AddDays(30),
            ExpiresAt = DateTime.UtcNow.AddMinutes(1),
            CreatedAt = DateTime.UtcNow,
            LastActive = DateTime.UtcNow,
            Revoked = false
        };

        // Save refresh token to user
        await userSessionService.CreateAsync(session);
        throw ResponseFactory.Create<OkResponse<LoginResponseDTO>>(
            new LoginResponseDTO(accessToken, refreshToken, session.Id), ["Login successful"]);
    }

    /// <summary>
    /// Verifies two-factor authentication code and completes the login process.
    /// </summary>
    /// <param name="dto">Verification DTO containing the 2FA code.</param>
    /// <returns>Access and refresh tokens upon successful verification.</returns>
    /// <response code="200">2FA verification successful.</response>
    /// <response code="400">Invalid or expired code.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost("verify-2fa")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaDTO dto)
    {
        User user = await CheckAccessFeatures([]);

        bool isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultEmailProvider,
            dto.Code);

        if (!isValid)
            throw ResponseFactory.Create<BadRequestResponse>(["Invalid or expired code"]);

        // Generate both tokens
        string accessToken =
            authService.GenerateJwtToken(user, Request.Headers["X-Device-Name"].ToString() ?? "Unknown device");
        string refreshToken = authService.GenerateRefreshToken();

        UserSession session = new()
        {
            UserId = user.Id,
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
        await userSessionService.CreateAsync(session);
        throw ResponseFactory.Create<OkResponse<RefreshTokenResponse>>(
            new RefreshTokenResponse(accessToken, refreshToken), ["Login successful"]);
    }

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token.
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
        UserSession session = await userSessionService.GetValidatedByRefreshTokenAsync(refreshHash);
        await userSessionService.UpdateLastActiveAsync(session);
        string newAccessToken = authService.GenerateJwtToken(session.User, session.DeviceName);
        throw ResponseFactory.Create<OkResponse<RefreshTokenResponse>>(
            new RefreshTokenResponse(newAccessToken, refreshToken), ["Token refreshed successfully"]);
    }

    /// <summary>
    /// Initiates an email change request by sending a confirmation link to the new email address.
    /// </summary>
    /// <param name="newEmail">The new email address.</param>
    /// <returns>Success response indicating email was sent.</returns>
    /// <response code="200">Confirmation email sent.</response>
    /// <response code="401">User not authenticated.</response>
    [Authorize]
    [HttpPost("request-email-change")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RequestEmailChange([FromBody] string newEmail)
    {
        User user = await CheckAccessFeatures([]);

        string token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);

        string confirmationLink =
            $"{frontEndUrl}/confirm-email-change?userId={user.Id}&email={newEmail}&token={Uri.EscapeDataString(token)}";

        await emailSenderService.SendEmailAsync(
            newEmail,
            MailGunTemplates.confirmEmail,
            new Dictionary<string, string>
            {
                { "link", confirmationLink }
            }
        );

        throw ResponseFactory.Create<OkResponse>(["Email change token sent to new email address"]);
    }

    /// <summary>
    /// Confirms email change using the token sent to the new email address.
    /// </summary>
    /// <param name="changeDTO">Email change confirmation data including user ID, new email, and token.</param>
    /// <returns>Success response upon email change.</returns>
    /// <response code="200">Email successfully changed.</response>
    /// <response code="400">Invalid token or email.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost("confirm-email-change")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDTO changeDTO)
    {
        User user = await CheckAccessFeatures([]);

        IdentityResult result = await userManager.ChangeEmailAsync(user, changeDTO.email, changeDTO.token);

        if (!result.Succeeded)
            throw ResponseFactory.Create<BadRequestResponse>(result.Errors.Select(e => e.ToString()).ToArray()!);

        await userManager.SetUserNameAsync(user, changeDTO.email);

        throw ResponseFactory.Create<OkResponse>(["Email successfully changed"]);
    }

    /// <summary>
    /// Changes the user's password after verifying the old password.
    /// </summary>
    /// <param name="dto">Password change data including old password, new password, and optional 2FA token.</param>
    /// <returns>Success response upon password change.</returns>
    /// <response code="200">Password successfully changed.</response>
    /// <response code="400">Invalid old password or 2FA token.</response>
    /// <response code="401">User not authenticated.</response>
    [Authorize]
    [HttpPost("confirm-password-change")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmPasswordChange([FromBody] ConfirmPasswordChangeDTO dto)
    {
        User user = await CheckAccessFeatures([]);

        if (user is { Enabled2FA: true, Email: not null })
        {
            bool isValid = await userManager.VerifyUserTokenAsync(
                user,
                userManager.Options.Tokens.PasswordResetTokenProvider,
                "ResetPassword",
                dto.Token
            );

            if (!isValid)
                throw ResponseFactory.Create<BadRequestResponse>();
        }

        IdentityResult result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

        if (!result.Succeeded)
            throw ResponseFactory.Create<BadRequestResponse>(result.Errors.Select(e => e.ToString()).ToArray()!,
                ["Password change failed"]);

        await signInManager.RefreshSignInAsync(user);
        throw ResponseFactory.Create<OkResponse>(["Password successfully changed"]);
    }

    /// <summary>
    /// Initiates a password change request. If 2FA is enabled, sends a reset token via email.
    /// </summary>
    /// <returns>Success response indicating reset instructions were sent (if applicable).</returns>
    /// <response code="200">Password reset link sent (if 2FA enabled).</response>
    /// <response code="401">User not authenticated.</response>
    [Authorize]
    [HttpPost("request-password-change")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RequestPasswordChange()
    {
        User user = await CheckAccessFeatures([]);

        if (user.Email == null || !await userManager.GetTwoFactorEnabledAsync(user))
            throw ResponseFactory.Create<OkResponse>(["Password reset link sent to your email."]);

        string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        string resetLink = $"{frontEndUrl}/approve-change/{resetToken}";
        await emailSenderService.SendEmailAsync(
            user.Email,
            MailGunTemplates.passwordRecovery,
            new Dictionary<string, string>
            {
                { "link", resetLink }
            }
        );
        throw ResponseFactory.Create<OkResponse>(["2FA is enabled. Please verify the token before changing password."]);
    }

    /// <summary>
    /// Revokes a specific user session by session ID.
    /// </summary>
    /// <param name="sessionId">The session ID to revoke.</param>
    /// <returns>Success response upon session revocation.</returns>
    /// <response code="200">Session revoked successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Session does not belong to the authenticated user.</response>
    /// <response code="404">Session not found.</response>
    [Authorize]
    [HttpPost("sessions/{sessionId:int}/revoke")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeSessionAsync(int sessionId)
    {
        User user = await CheckAccessFeatures([]);
        UserSession session = await userSessionService.GetByIdValidatedAsync(sessionId);
        if (session.UserId == user.Id)
            await userSessionService.RevokeSessionAsync(session);
        else
            throw ResponseFactory.Create<ForbiddenResponse>(["Invalid session"]);
        throw ResponseFactory.Create<OkResponse>(["Session revoked successfully"]);
    }


    /// <summary>
    /// Revokes all active sessions for the authenticated user.
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
        User user = await CheckAccessFeatures([]);
        await userSessionService.RevokeAllUserSessionsAsync(user.Id);
        throw ResponseFactory.Create<OkResponse>(["All sessions revoked successfully"]);
    }

    /// <summary>
    /// Retrieves all active sessions for the authenticated user.
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
        User user = await CheckAccessFeatures([]);
        IEnumerable<ActiveSessionDTO> sessions = (await userSessionService.GetAllByUserIdAsync(user.Id)).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ActiveSessionDTO>>>(sessions
            , ["Sessions retrieved successfully"]);
    }
}