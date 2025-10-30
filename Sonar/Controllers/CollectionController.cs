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

public class CollectionController<T>(UserManager<User> userManager, ICollectionService<T> collectionService, IShareService shareService)
    : ShareController<T>(userManager, shareService) where T : Collection
{
    [HttpPut("{collectionId:int}/visibility")]
    [Authorize]
    public async Task<IActionResult> UpdateVisibilityStatus(int collectionId, int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
        await collectionService.UpdateVisibilityStatusAsync(collectionId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>([$"{nameof(T)} visibility status was changed successfully"]);
    }
}