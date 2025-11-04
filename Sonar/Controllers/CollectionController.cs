using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Enums;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("{collectionId:int}")]
public class CollectionController<T>(UserManager<User> userManager, ICollectionService<T> collectionService)
    : BaseController(userManager) where T : Collection
{
    // TODO: write XML comments and returnType attributes
    [HttpPut("visibility")]
    [Authorize]
    public async Task<IActionResult> UpdateVisibilityStatus([FromRoute] int collectionId,
        [FromQuery] int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
        await collectionService.UpdateVisibilityStatusAsync(collectionId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>([$"{nameof(T)} visibility status was changed successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPost("toggle-favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int collectionId)
    {
        User user = await CheckAccessFeatures([]);
        await collectionService.ToggleFavoriteAsync(user.LibraryId, collectionId);
        throw ResponseFactory.Create<OkResponse>([$"{nameof(T)} favorite status was toggled successfully"]);
    }
}