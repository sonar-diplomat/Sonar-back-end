using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
using Application.Exception;
using Entities.Enums;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Sonar.Controllers.UserCore;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IConfiguration configuration,
    IUserService userService,
    IEmailSenderService emailSenderService
)
    : ControllerBase
{
    private readonly string frontEndUrl = configuration["FrontEnd-Url"]!;

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDTO model)
    {
        try
        {
            User user = await userService.CreateUserShellAsync(model);
            IdentityResult result = await userManager.CreateAsync(user, model.Password);
            return Ok(result.Succeeded ? new { message = "Registration successful" } : new { message = "Registration failed" });
        }
        catch (Exception)
        {
            throw AppExceptionFactory.Create<BadRequestException>(["Request contents were not sufficient to complete registration"]);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string userIdentifier, string password)
    {
        User? user = await userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == userIdentifier || u.Email == userIdentifier);

        // TODO: Тимоша вызовет исключение
        if (user == null) return BadRequest("Invalid credentials");

        SignInResult result = await signInManager.CheckPasswordSignInAsync(
            user, password, false);

        if (result.Succeeded)
        {
            if (user.Enabled2FA)
            {
                string code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                await emailSenderService.SendEmailAsync(
                    user.Email!,
                    MailGunTemplates.twoFA,
                    new Dictionary<string, string>
                    {
                        { "code", code }
                    });

                return Ok(new
                {
                    require2FA = true,
                    message = "Verification code sent to your email."
                });
            }

            // Generate both tokens
            string accessToken = GenerateJwtToken(user);
            RefreshToken refreshToken = GenerateRefreshToken();
            // Save refresh token to user
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);
            return Ok(new
            {
                accessToken,
                refreshToken = refreshToken.Token
            });
        }

        // TODO: Тимоша вызовет исключение
        throw new NotImplementedException();
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaDTO dto)
    {
        User? user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return BadRequest("Invalid credentials");

        bool isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultEmailProvider,
            dto.Code);

        if (!isValid)
            return BadRequest("Invalid or expired code");


        // Generate both tokens
        string accessToken = GenerateJwtToken(user);
        RefreshToken refreshToken = GenerateRefreshToken();
        // Save refresh token to user
        user.RefreshTokens.Add(refreshToken);
        await userManager.UpdateAsync(user);
        return Ok(new
        {
            accessToken,
            refreshToken = refreshToken.Token
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetMailChangeToken([FromBody] string newEmail)
    {
        // TODO: Add login
        User? user = new(); // await userService.GetUserByLogin("login");  // userLogin
        if (user == null)
            // TODO: Тимоша вызывает исключение
            throw new Exception("User not found");

        try
        {
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
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }

        return Ok(new { message = "Confirm your email change" });
    }


    [HttpPost("confirm-email-change")]
    public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string token)
    {
        User? user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest("User not found");

        IdentityResult result = await userManager.ChangeEmailAsync(user, email, token);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await userManager.SetUserNameAsync(user, email);

        return Ok(new { message = "Email successfully changed" });
    }

    // TODO: Consider the logic of how we can change the password with two-factor authentication.
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        User? user = await userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized("User not found");
        if (user.Email != null && await userManager.GetTwoFactorEnabledAsync(user))
        {
            string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            string resetLink = $"{frontEndUrl}/confirm-change-password/{resetToken}";


            await emailSenderService.SendEmailAsync(
                user.Email,
                MailGunTemplates.passwordRecovery,
                new Dictionary<string, string>
                {
                    { "link", resetLink }
                }
            );

            return Ok(new
            {
                message = "2FA is enabled. Please verify the token before changing password."
            });
        }

        IdentityResult result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            IEnumerable<string> errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { message = "Password change failed", errors });
        }

        await signInManager.RefreshSignInAsync(user);

        return Ok(new { message = "Password successfully changed" });
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDTO refreshTokenDto)
    {
        User? user = await userManager.Users
            .SingleOrDefaultAsync(u => u.RefreshTokens
                .Any(t => t.Token == refreshTokenDto.Token &&
                          t.ExpiryDate > DateTime.UtcNow));
        if (user == null)
            return BadRequest("Invalid token");
        string newAccessToken = GenerateJwtToken(user);
        RefreshToken newRefreshToken = GenerateRefreshToken();

        user.RefreshTokens.RemoveAll(t => t.Token == refreshTokenDto.Token);
        user.RefreshTokens.Add(newRefreshToken);
        await userManager.UpdateAsync(user);
        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken.Token
        });
    }

    private RefreshToken GenerateRefreshToken()
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
    }

    private string GenerateJwtToken(User user)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Login)
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
