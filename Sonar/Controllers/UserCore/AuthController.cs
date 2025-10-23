using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs.Auth;
using Application.Exception;
using Entities.Enums;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
    IUserSessionService userSessionService
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
            return Ok(result.Succeeded ? new BaseResponse<string>("Registration successfull") : new BaseResponse<string>($"Registration failed {string.Join("\n",result.Errors.Select(e=>e.Description).ToList())}"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery]string userIdentifier, [FromQuery]string password)
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
            return Ok(new BaseResponse<bool>(true, "2FA code sent to email"));
        }

        // Generate both tokens
        string accessToken = GenerateJwtToken(user);
        string refreshToken = GenerateRefreshToken();

        UserSession session = new()
        {
            UserId = user.Id,
            DeviceName = Request.Headers["X-Device-Name"].ToString() ?? "Unknown device",
            UserAgent = Request.Headers["User-Agent"].ToString() ?? "Unknown",
            IPAddress = HttpContext.Connection.RemoteIpAddress!,
            RefreshTokenHash = ComputeSha256(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            LastActive = DateTime.UtcNow,
            Revoked = false
        };

        // Save refresh token to user
        await userSessionService.CreateAsync(session);
        return Ok(new BaseResponse<LoginResponseDTO>(new LoginResponseDTO(accessToken, refreshToken, session.Id), "Login successful"));
    }

    private static string ComputeSha256(string input)
    {
        using SHA256 sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaDTO dto)
    {
        User? user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null) throw AppExceptionFactory.Create<BadRequestException>([$"Invalid credentials"]);


        bool isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultEmailProvider,
            dto.Code);

        if (!isValid)
            throw AppExceptionFactory.Create<BadRequestException>([$"Invalid or expired code"]);


        // Generate both tokens
        string accessToken = GenerateJwtToken(user);
        string refreshToken = GenerateRefreshToken();

        UserSession session = new()
        {
            UserId = user.Id,
            DeviceName = Request.Headers["X-Device-Name"].ToString() ?? "Unknown device",
            UserAgent = Request.Headers["User-Agent"].ToString() ?? "Unknown",
            IPAddress = HttpContext.Connection.RemoteIpAddress!,
            RefreshTokenHash = ComputeSha256(refreshToken),
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

    [Authorize]
    [HttpPost("request-email-change")]
    public async Task<IActionResult> RequestEmailChange([FromBody] string newEmail)
    {
        User user = await GetUserByJwtAsync();

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
    public async Task<IActionResult> ConfirmEmailChange([FromBody]ConfigmEmailChangeDTO changeDTO)
    {
        User? user = await userManager.FindByIdAsync(changeDTO.userId);
        if (user == null)
            throw AppExceptionFactory.Create<UserNotFoundException>();

        IdentityResult result = await userManager.ChangeEmailAsync(user, changeDTO.email, changeDTO.token);

        if (!result.Succeeded)
            throw AppExceptionFactory.Create<BadRequestException>(result.Errors.Select(e => e.ToString()).ToArray()!);

        await userManager.SetUserNameAsync(user, changeDTO.email);

        return Ok(new BaseResponse<string>("Email successfully changed"));
    }

    [Authorize]
    [HttpPost("confirm-password-change")]
    public async Task<IActionResult> ConfirmPasswordChange([FromBody] ConfirmPasswordChangeDTO dto)
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
    [HttpPost("request-password-change")]
    public async Task<IActionResult> RequestPasswordChange()
    {
        User? user = await userManager.GetUserAsync(User);
        if (user == null) throw AppExceptionFactory.Create<UserNotFoundException>();

        //// ??????? TODO
        //if (user.Email != null && await userManager.GetTwoFactorEnabledAsync(user))
        //{
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

        //    return Ok(new BaseResponse<string>("2FA is enabled. Please verify the token before changing password."));
        //}
        ////

        return Ok(new BaseResponse<string>( "Password reset link sent to your email." ));
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        string refreshHash = ComputeSha256(refreshToken);
        UserSession session = await userSessionService.GetValidatedByRefreshTokenAsync(refreshHash);
        await userSessionService.UpdateLastActiveAsync(session);

        string newAccessToken = GenerateJwtToken(session.User);

        return Ok(new BaseResponse<RefreshTokenResponse>(new RefreshTokenResponse(newAccessToken, refreshToken), "Token refreshed successfully"));
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
            return Forbid("Session is not your!");

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
            "Sessions retrieved successfully")
        );
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private string GenerateJwtToken(User user)
    {
        Claim[] claims =
        [
           new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new(ClaimTypes.NameIdentifier, user.Login),  // ✅
    new(ClaimTypes.Name, user.UserName == null ? user.Login : user.UserName),
        ];
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
