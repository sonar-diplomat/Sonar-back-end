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
    private readonly UserManager<User> userManager = userManager;

    /// <summary>
    /// Registers a new user account in the system.
    /// </summary>
    /// <param name="model">User registration data including username, email, password, and personal information.</param>
    /// <returns>Success response upon successful registration.</returns>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Invalid registration data.</response>
    /// <response code="409">User with this login or email already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(UserRegisterDTO model)
    {
        if (!ModelState.IsValid)
        {
            List<string> validationErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrEmpty(m))
                .ToList();
            
            throw ResponseFactory.Create<BadRequestResponse>(validationErrors.ToArray());
        }
        
        try
        {
            User user = await userService.CreateUserShellAsync(model);
            IdentityResult result = await userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Отправляем письмо подтверждения email
                if (!string.IsNullOrEmpty(user.Email))
                {
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    
                    await emailSenderService.SendEmailAsync(
                        user.Email,
                        MailGunTemplates.confirmEmail,
                        new Dictionary<string, string>
                        {
                            { "route", "confirm-email" },
                            { "linkParam_email", user.Email },
                            { "linkParam_token", token }
                        }
                    );
                }
                
                throw ResponseFactory.Create<OkResponse>(["Registration successful. Please check your email to confirm your account."]);
            }
            
            List<string> conflictErrors = new();
            List<string> identityValidationErrors = new();
            
            foreach (IdentityError error in result.Errors)
            {
                string message = error.Code switch
                {
                    "DuplicateUserName" or "DuplicateNormalizedUserName" => "User with this username already exists",
                    "DuplicateEmail" => "Email is already in use",
                    "PasswordTooShort" => $"Password is too short. Minimum length: {userManager.Options.Password.RequiredLength} characters",
                    "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character",
                    "PasswordRequiresDigit" => "Password must contain at least one digit",
                    "PasswordRequiresLower" => "Password must contain at least one lowercase letter",
                    "PasswordRequiresUpper" => "Password must contain at least one uppercase letter",
                    "InvalidEmail" => "Invalid email address",
                    "InvalidUserName" => "Invalid username",
                    _ => error.Description
                };
                
                if (error.Code is "DuplicateUserName" or "DuplicateNormalizedUserName" or "DuplicateEmail")
                    conflictErrors.Add(message);
                else
                    identityValidationErrors.Add(message);
            }
            
            if (conflictErrors.Count > 0)
                throw ResponseFactory.Create<ConflictResponse>(conflictErrors.ToArray());
            
            if (identityValidationErrors.Count > 0)
                throw ResponseFactory.Create<BadRequestResponse>(identityValidationErrors.ToArray());
            
            throw ResponseFactory.Create<BadRequestResponse>(
                result.Errors.Select(e => e.Description).ToArray());
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("duplicate") == true ||
                                             dbEx.InnerException?.Message.Contains("unique") == true ||
                                             dbEx.InnerException?.Message.Contains("UserNameIndex") == true ||
                                             dbEx.InnerException?.Message.Contains("EmailIndex") == true)
        {
            string errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            
            if (errorMessage.Contains("UserName") || errorMessage.Contains("UserNameIndex"))
                throw ResponseFactory.Create<ConflictResponse>(["User with this username already exists"]);
            
            if (errorMessage.Contains("Email") || errorMessage.Contains("EmailIndex"))
                throw ResponseFactory.Create<ConflictResponse>(["Email is already in use"]);
            
            throw ResponseFactory.Create<ConflictResponse>(["User with such data already exists"]);
        }
    }

    /// <summary>
    /// Authenticates a user and returns JWT access and refresh tokens.
    /// </summary>
    /// <param name="userIdentifier">Username or email address.</param>
    /// <param name="password">User password.</param>
    /// <returns>Login response containing access token, refresh token, and session ID. If 2FA is enabled, returns confirmation message.</returns>
    /// <response code="200">Login successful or 2FA code sent.</response>
    /// <response code="400">User not found or email not confirmed.</response>
    /// <response code="417">Invalid credentials.</response>
    /// <remarks>
    /// Requires X-Device-Name header for session tracking.
    /// Email must be confirmed before login. If 2FA is enabled, a verification code will be sent to the user's email.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(OkResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExpectationFailedResponse), StatusCodes.Status417ExpectationFailed)]
    public async Task<IActionResult> Login(string userIdentifier, string password)
    {
        User? user = await userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == userIdentifier || u.Email == userIdentifier);

        if (user == null) throw ResponseFactory.Create<BadRequestResponse>([$"User {userIdentifier} not found"]);

        // Проверяем, подтвержден ли email
        if (!user.EmailConfirmed)
            throw ResponseFactory.Create<BadRequestResponse>(["Please confirm your email address before logging in. Check your email for the confirmation link."]);

        SignInResult result = await signInManager.CheckPasswordSignInAsync(
            user!, password, false);

        if (!result.Succeeded) throw ResponseFactory.Create<ExpectationFailedResponse>();

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
            ExpiresAt = DateTime.UtcNow.AddDays(30),
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
    /// <response code="400">Invalid or expired code, or email not confirmed.</response>
    /// <response code="401">User not authenticated.</response>
    /// <remarks>
    /// Email must be confirmed before completing 2FA verification.
    /// </remarks>
    [HttpPost("verify-2fa")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaDTO dto)
    {
        User user = await CheckAccessFeatures([]);

        // Проверяем, подтвержден ли email
        if (!user.EmailConfirmed)
            throw ResponseFactory.Create<BadRequestResponse>(["Please confirm your email address before logging in. Check your email for the confirmation link."]);

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

        await emailSenderService.SendEmailAsync(
            newEmail,
            MailGunTemplates.confirmEmail,
            new Dictionary<string, string>
            {
                { "route", "confirm-email-change" },
                { "linkParam_userId", user.Id.ToString() },
                { "linkParam_email", newEmail },
                { "linkParam_token", token }
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
    /// Confirms email address during registration using the token sent to the user's email.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="token">The email confirmation token.</param>
    /// <returns>Success response upon email confirmation.</returns>
    /// <response code="200">Email successfully confirmed.</response>
    /// <response code="400">Invalid token or email.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("confirm-email")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw ResponseFactory.Create<BadRequestResponse>(["Email is required"]);

        User? user = await userManager.FindByEmailAsync(email);
        if (user == null)
            throw ResponseFactory.Create<NotFoundResponse>(["User not found"]);

        IdentityResult result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            throw ResponseFactory.Create<BadRequestResponse>(result.Errors.Select(e => e.Description).ToArray());

        throw ResponseFactory.Create<OkResponse>(["Email successfully confirmed"]);
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
        await emailSenderService.SendEmailAsync(
            user.Email,
            MailGunTemplates.passwordRecovery,
            new Dictionary<string, string>
            {
                { "route", $"approve-change/{resetToken}" }
            }
        );
        throw ResponseFactory.Create<OkResponse>(["2FA is enabled. Please verify the token before changing password."]);
    }

    /// <summary>
    /// Initiates a password reset request by sending a reset token to the user's email.
    /// </summary>
    /// <param name="dto">Forgot password DTO containing the user's email address.</param>
    /// <returns>Success response indicating reset instructions were sent.</returns>
    /// <response code="200">Password reset link sent to email (if user exists).</response>
    /// <response code="400">Invalid email format.</response>
    /// <remarks>
    /// This endpoint does not reveal whether the email exists in the system for security reasons.
    /// Always returns success message even if email is not found.
    /// </remarks>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw ResponseFactory.Create<BadRequestResponse>(["Email is required"]);

        User? user = await userManager.FindByEmailAsync(dto.Email);
        
        // Для безопасности всегда возвращаем успешный ответ, даже если пользователь не найден
        if (user == null || !user.EmailConfirmed)
        {
            // Не раскрываем информацию о существовании пользователя
            throw ResponseFactory.Create<OkResponse>(["If the email exists and is confirmed, a password reset link has been sent."]);
        }

        string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        
        await emailSenderService.SendEmailAsync(
            user.Email!,
            MailGunTemplates.passwordRecovery,
            new Dictionary<string, string>
            {
                { "route", "reset-password" },
                { "linkParam_email", user.Email! },
                { "linkParam_token", resetToken }
            }
        );

        throw ResponseFactory.Create<OkResponse>(["If the email exists and is confirmed, a password reset link has been sent."]);
    }

    /// <summary>
    /// Resets the user's password using the token sent to their email.
    /// </summary>
    /// <param name="dto">Reset password DTO containing email, token, and new password.</param>
    /// <returns>Success response upon password reset.</returns>
    /// <response code="200">Password successfully reset.</response>
    /// <response code="400">Invalid token, email, or password requirements not met.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.NewPassword))
            throw ResponseFactory.Create<BadRequestResponse>(["Email, token, and new password are required"]);

        User? user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw ResponseFactory.Create<NotFoundResponse>(["User not found"]);

        if (!user.EmailConfirmed)
            throw ResponseFactory.Create<BadRequestResponse>(["Email address must be confirmed before resetting password"]);

        IdentityResult result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (!result.Succeeded)
            throw ResponseFactory.Create<BadRequestResponse>(result.Errors.Select(e => e.Description).ToArray());

        throw ResponseFactory.Create<OkResponse>(["Password successfully reset"]);
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