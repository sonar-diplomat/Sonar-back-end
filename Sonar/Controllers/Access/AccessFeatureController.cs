using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Enums;
using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessFeatureController(UserManager<User> userManager, IAccessFeatureService accessFeatureService, IUserService userService) : BaseController(userManager)
{
    [HttpPost("assign/{userId:int}")]
    public async Task<IActionResult> AssignAccessFeatures(int userId, [FromBody] int[] accessFeatureIds)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await userService.AssignAccessFeaturesAsync(userId, accessFeatureIds);
        throw ResponseFactory.Create<OkResponse>(["Access feature was assigned to user successfully"]);
    }

    [HttpPost("revoke/{userId:int}")]
    public async Task<IActionResult> RevokeAccessFeatures(int userId, [FromBody] int[] accessFeatureIds)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await userService.RevokeAccessFeaturesAsync(userId, accessFeatureIds);
        throw ResponseFactory.Create<OkResponse>(["Access feature was revoked from user successfully"]);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccessFeatures()
    {
        IEnumerable<AccessFeature> accessFeatures = (await accessFeatureService.GetAllAsync()).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<AccessFeature>>>(accessFeatures, ["Access features retrieved successfully"]);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccessFeatureById(int id)
    {
        AccessFeature accessFeature = await accessFeatureService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<AccessFeature>>(accessFeature, ["Access feature retrieved successfully"]);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetAccessFeaturesByUserId(int userId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        ICollection<AccessFeature> features = await accessFeatureService.GetUserFeaturesByIdAsync(userId);
        throw ResponseFactory.Create<OkResponse<ICollection<AccessFeature>>>(features, ["User access features were retrieved successfully"]);
    }
}
