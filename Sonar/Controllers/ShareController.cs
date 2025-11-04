using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Response;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

public class ShareController<T>(UserManager<User> userManager, IShareService shareService)
    : BaseController(userManager) where T : class
{
    // TODO: write XML comments and returnType attributes
    [HttpGet("{entityId:int}/link")]
    public async Task<IActionResult> ShareLink(int entityId)
    {
        string link = await shareService.GenerateLinkAsync<T>(entityId);
        throw ResponseFactory.Create<OkResponse<string>>(link, ["Link generated successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("{entityId:int}/qr")]
    public async Task<IActionResult> ShareQr(int entityId)
    {
        string link = await shareService.GenerateLinkAsync<T>(entityId);
        string svgQr = await shareService.GenerateQrCode(link);
        throw ResponseFactory.Create<OkResponse<string>>(svgQr, ["QR code generated successfully"]);
    }
}