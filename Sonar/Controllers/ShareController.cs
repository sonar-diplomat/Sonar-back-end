using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Response;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

public class ShareController<T>(UserManager<User> userManager, IShareService shareService)
    : BaseController(userManager) where T : class
{
    /// <summary>
    /// Generates a shareable link for an entity (user, album, playlist, etc.).
    /// </summary>
    /// <param name="entityId">The ID of the entity to generate a share link for.</param>
    /// <returns>The generated shareable link URL.</returns>
    /// <response code="200">Share link generated successfully.</response>
    /// <response code="404">Entity not found.</response>
    [HttpGet("{entityId:int}/link")]
    [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShareLink(int entityId)
    {
        string link = await shareService.GenerateLinkAsync<T>(entityId);
        throw ResponseFactory.Create<OkResponse<string>>(link, ["Link generated successfully"]);
    }

    /// <summary>
    /// Generates a QR code for sharing an entity (user, album, playlist, etc.).
    /// </summary>
    /// <param name="entityId">The ID of the entity to generate a QR code for.</param>
    /// <returns>The generated QR code as an SVG string.</returns>
    /// <response code="200">QR code generated successfully.</response>
    /// <response code="404">Entity not found.</response>
    [HttpGet("{entityId:int}/qr")]
    [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShareQr(int entityId)
    {
        string link = await shareService.GenerateLinkAsync<T>(entityId);
        string svgQr = await shareService.GenerateQrCode(link);
        throw ResponseFactory.Create<OkResponse<string>>(svgQr, ["QR code generated successfully"]);
    }
}