using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Enums;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

public class CollectionController<T>(UserManager<User> userManager, ICollectionService<T> collectionService) : BaseController(userManager) where T : Collection
{
    [HttpGet("{collectionId:int}/share-link")]
    public async Task<IActionResult> ShareLink(int collectionId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{collectionId:int}/share-qr")]
    public async Task<IActionResult> ShareQr(int collectionId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{collectionId:int}/visibility")]
    [Authorize]
    public async Task<IActionResult> UpdateVisibilityStatus(int collectionId, int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
        await collectionService.UpdateVisibilityStatusAsync(collectionId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>([$"{nameof(T)} visibility status was changed successfully"]);
    }
}