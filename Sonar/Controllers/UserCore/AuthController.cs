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

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDTO model)
    {
        User user = await userService.CreateUserShellAsync(model);
        IdentityResult result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
            throw ResponseFactory.Create<OkResponse>(["Registration successfull"]);
        throw ResponseFactory.Create<BadRequestResponse>(result.Errors.Select(e => e.Description.ToString()).ToArray()!);
    }

    [HttpPost("login")]
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

    [HttpPost("verify-2fa")]
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

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        string refreshHash = authService.ComputeSha256(refreshToken);
        UserSession session = await userSessionService.GetValidatedByRefreshTokenAsync(refreshHash);
        await userSessionService.UpdateLastActiveAsync(session);
        string newAccessToken = authService.GenerateJwtToken(session.User, session.DeviceName);
        throw ResponseFactory.Create<OkResponse<RefreshTokenResponse>>(
            new RefreshTokenResponse(newAccessToken, refreshToken), ["Token refreshed successfully"]);
    }

    [Authorize]
    [HttpPost("request-email-change")]
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

    [HttpPost("confirm-email-change")]
    public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDTO changeDTO)
    {
        User user = await CheckAccessFeatures([]);

        IdentityResult result = await userManager.ChangeEmailAsync(user, changeDTO.email, changeDTO.token);

        if (!result.Succeeded)
            throw ResponseFactory.Create<BadRequestResponse>(result.Errors.Select(e => e.ToString()).ToArray()!);

        await userManager.SetUserNameAsync(user, changeDTO.email);

        throw ResponseFactory.Create<OkResponse>(["Email successfully changed"]);
    }

    [Authorize]
    [HttpPost("confirm-password-change")]
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

    [Authorize]
    [HttpPost("request-password-change")]
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

    [Authorize]
    [HttpPost("sessions/{sessionId:int}/revoke")]
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


    [Authorize]
    [HttpPost("sessions/revoke-all")]
    public async Task<IActionResult> RevokeAllSessions()
    {
        User user = await CheckAccessFeatures([]);
        await userSessionService.RevokeAllUserSessionsAsync(user.Id);
        throw ResponseFactory.Create<OkResponse>(["All sessions revoked successfully"]);
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        User user = await CheckAccessFeatures([]);
        IEnumerable<ActiveSessionDTO> sessions = (await userSessionService.GetAllByUserIdAsync(user.Id)).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ActiveSessionDTO>>>(sessions
            , ["Sessions retrieved successfully"]);
    }
}