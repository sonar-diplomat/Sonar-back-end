using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Interfaces.Service.Utilities;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Enums;
using Entities.Models.UserCore;
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
    AppExceptionFactory appExceptionFactory,
    IEmailSenderService emailSenderService)
    : ControllerBase
{
    private readonly AppExceptionFactory appExceptionFactory = appExceptionFactory;

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDTO model)
    {
        try
        {
            User user = await userService.CreateUserShellAsync(model);
            IdentityResult result = await userManager.CreateAsync(user, model.Password);
        }
        catch (Exception e)
        {
            // TODO: Тимоша вызовет исключение
            // throw appExceptionFactory.CreateBadRequest();
        }

        return Ok(new { message = "Registration successful" });
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


    [HttpPost("2fa")]
    public async Task<IActionResult> TwoFactorAuthentication(string email)
    {
        try
        {
            string code = "191981";
            await emailSenderService.SendEmailAsync(
                email,
                MailGunTemplates.twoFA,
                new Dictionary<string, string>
                {
                    { "code", code }
                });
        }
        catch (Exception e)
        {
            throw new NotImplementedException();
        }

        return Ok(new { message = "2FA code sent" });
    }

    [HttpPost("verify/{code}")]
    public async Task<IActionResult> VerifyTwoFactorAuthentication(string code)
    {
        throw new NotImplementedException();
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

        // Remove old refresh token
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