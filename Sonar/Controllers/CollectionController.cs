using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Response;
using Entities.Enums;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("{collectionId:int}")]
public class CollectionController<T>(UserManager<User> userManager, ICollectionService<T> collectionService, IShareService shareService)
    : BaseController(userManager) where T : Collection
{
    /// <summary>
    /// Updates the visibility status of a collection.
    /// </summary>
    /// <param name="collectionId">The ID of the collection to update visibility for.</param>
    /// <param name="visibilityStatusId">The ID of the new visibility status.</param>
    /// <returns>Success response upon visibility status update.</returns>
    /// <response code="200">Visibility status updated successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ManageContent' access feature.</response>
    /// <response code="404">Collection or visibility status not found.</response>
    /// <remarks>
    /// Requires 'ManageContent' access feature.
    /// </remarks>
    [HttpPut("visibility")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVisibilityStatus([FromRoute] int collectionId,
        [FromQuery] int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
        await collectionService.UpdateVisibilityStatusAsync(collectionId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>([$"{nameof(T)} visibility status was changed successfully"]);
    }

    /// <summary>
    /// Toggles the favorite status of a collection in the user's library.
    /// </summary>
    /// <param name="collectionId">The ID of the collection to toggle favorite status for.</param>
    /// <returns>Success response upon toggling favorite status.</returns>
    /// <response code="200">Favorite status toggled successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Collection not found.</response>
    [HttpPost("toggle-favorite")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFavorite(int collectionId)
    {
        User user = await CheckAccessFeatures([]);
        await collectionService.ToggleFavoriteAsync(user.LibraryId, collectionId);
        throw ResponseFactory.Create<OkResponse>([$"{nameof(T)} favorite status was toggled successfully"]);
    }
    
    /// <summary>
    /// Generates a shareable link for an entity (user, album, playlist, etc.).
    /// </summary>
    /// <param name="entityId">The ID of the entity to generate a share link for.</param>
    /// <returns>The generated shareable link URL.</returns>
    /// <response code="200">Share link generated successfully.</response>
    /// <response code="404">Entity not found.</response>
    [HttpGet("link")]
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
    [HttpGet("qr")]
    [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShareQr(int entityId)
    {
        string link = await shareService.GenerateLinkAsync<T>(entityId);
        string svgQr = await shareService.GenerateQrCode(link);
        throw ResponseFactory.Create<OkResponse<string>>(svgQr, ["QR code generated successfully"]);
    }
}