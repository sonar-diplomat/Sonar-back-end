using System.Security.Cryptography;
using System.Text;
using Application.DTOs;
using Application.Exception;
using Application.Services.Distribution;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class AuthDistributorController(DistributorSessionService sessionService)
{
    [HttpDelete("session/{id}")]
    public async Task<IActionResult> TerminateSession(int id)
    {
        await sessionService.TerminateSessionAsync(id);
        return Ok(new BaseResponse<string>("Session terminated successfully"));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(DistributorAccountRegisterDTO dto)
    {
        if (await UserExists(dto.Username))
        {
            return BadRequest("Username already exists");
        }

        using var hmac = new HMACSHA512();
        var user = new DistributorAccount()
        {
            Username = dto.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            PasswordSalt = hmac.Key,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(string accountIdentifier, string password)
    {
        DistributorAccount account = null; //await distributorAccountService.GetByUsernameAsync(accountIdentifier);
        if (account == null)
            throw AppExceptionFactory.Create<UnauthorizedException>(["Invalid credentials"]);

        using HMACSHA512 hmac = new(account.PasswordSalt);
        byte[]? computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        if (!computedHash.SequenceEqual(account.PasswordHash))
            throw AppExceptionFactory.Create<UnauthorizedException>(["Invalid credentials"]);

        var token = CreateToken(user);
        return Ok(new { token });
    }
}
