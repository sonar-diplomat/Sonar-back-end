using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
using Application.Exception;
using Entities.Enums;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
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
        return Ok(result.Succeeded ? new BaseResponse<string>("Registration successfull") : new BaseResponse<string>("Registration failed"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string userIdentifier, string password)
    {
        User? user = await userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == userIdentifier || u.Email == userIdentifier);

        if (user == null) AppExceptionFactory.Create<BadRequestException>([$"User {userIdentifier} not found"]);


        SignInResult result = await signInManager.CheckPasswordSignInAsync(
            user!, password, false);

        if (!result.Succeeded) AppExceptionFactory.Create<ExpectationFailedException>();

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


            // TODO: What does the frontend need to proceed with data?
            return Ok(new BaseResponse<string>("2FA code sent to email"));
        }

        // Generate both tokens
        string accessToken = authService.GenerateJwtToken(user.Email, user.Login);
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
        user.UserSessions.Add(session);
        await userManager.UpdateAsync(user);
        return Ok(new BaseResponse<(string, string, int)>((accessToken, refreshToken, session.Id), "Login successful"));
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaDTO dto)
    {
        User? user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null) throw AppExceptionFactory.Create<BadRequestException>(["Invalid credentials"]);


        bool isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultEmailProvider,
            dto.Code);

        if (!isValid)
            throw AppExceptionFactory.Create<BadRequestException>(["Invalid or expired code"]);


        // Generate both tokens
        string accessToken = authService.GenerateJwtToken(user);
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
        user.UserSessions.Add(session);
        await userManager.UpdateAsync(user);
        return Ok(new
        {
            accessToken, refreshToken
        });
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        string refreshHash = authService.ComputeSha256(refreshToken);
        UserSession session = await userSessionService.GetValidatedByRefreshTokenAsync(refreshHash);
        await userSessionService.UpdateLastActiveAsync(session);

        string newAccessToken = authService.GenerateJwtToken(session.User.Email, session.User.Login);

        return Ok(new BaseResponse<(string, string)>((newAccessToken, refreshToken), "Token refreshed successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> GetMailChangeToken([FromBody] string newEmail)
    {
        User user = await GetUserByJwt();

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


        // TODO: <string> 
        return Ok(new BaseResponse<string>("Email change token sent to new email address"));
    }

    [HttpPost("confirm-email-change")]
    public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string token)
    {
        User? user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw AppExceptionFactory.Create<UserNotFoundException>();

        IdentityResult result = await userManager.ChangeEmailAsync(user, email, token);

        if (!result.Succeeded)
            throw AppExceptionFactory.Create<BadRequestException>(result.Errors.Select(e => e.ToString()).ToArray()!);

        await userManager.SetUserNameAsync(user, email);

        return Ok(new BaseResponse<string>("Email successfully changed"));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        User? user = await userManager.GetUserAsync(User);
        if (user == null)
            throw AppExceptionFactory.Create<UnauthorizedException>();

        if (user is { Enabled2FA: true, Email: not null })
        {
            bool isValid = await userManager.VerifyUserTokenAsync(
                user,
                userManager.Options.Tokens.PasswordResetTokenProvider,
                "ResetPassword",
                dto.Token
            );

            if (!isValid)
                throw AppExceptionFactory.Create<BadRequestException>();
        }


        IdentityResult result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

        if (!result.Succeeded)
            // TODO: Send erros to frontend
            // IEnumerable<string> errors = result.Errors.Select(e => e.Description);
            throw AppExceptionFactory.Create<BadRequestException>();

        await signInManager.RefreshSignInAsync(user);

        return Ok(new BaseResponse<string>("Password successfully changed"));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RequestPasswordChange()
    {
        User? user = await userManager.GetUserAsync(User);
        if (user == null) throw AppExceptionFactory.Create<UserNotFoundException>();

        // ??????? TODO
        if (user.Email != null && await userManager.GetTwoFactorEnabledAsync(user))
        {
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

            return Ok(new BaseResponse<string>("2FA is enabled. Please verify the token before changing password."));
        }
        //

        return Ok(new BaseResponse<string>("Password reset link sent to your email."));
    }

    [Authorize]
    [HttpPost("{sessionId:int}/revoke")]
    public async Task<IActionResult> RevokeSessionAsync(int sessionId)
    {
        User user = await CheckAccessFeatures([]);

        UserSession session = await userSessionService.GetByIdValidatedAsync(sessionId);

        await userSessionService.RevokeSessionAsync(session);

        return Ok(new BaseResponse<string>("Session revoked successfully"));
    }


    [Authorize]
    [HttpPost("sessions/revoke-all")]
    public async Task<IActionResult> RevokeAllSessions()
    {
        User user = await CheckAccessFeatures([]);

        await userSessionService.RevokeAllUserSessionsAsync(user.Id);

        return Ok(new BaseResponse<string>("All sessions revoked successfully"));
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        User user = await CheckAccessFeatures([]);

        return Ok(new BaseResponse<IEnumerable<ActiveUserSessionDTO>>( 
            await userSessionService.GetAllByUserIdAsync(user.Id),
            "Sessions retrieved successfully"));
    }
}
