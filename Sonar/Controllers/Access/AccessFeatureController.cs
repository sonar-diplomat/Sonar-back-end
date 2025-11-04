using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Access;
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
    // TODO: write XML comments and returnType attributes
    [HttpPost("assign/{userId:int}")]
    public async Task<IActionResult> AssignAccessFeatures(int userId, [FromBody] int[] accessFeatureIds)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await userService.AssignAccessFeaturesAsync(userId, accessFeatureIds);
        throw ResponseFactory.Create<OkResponse>(["Access feature was assigned to user successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPost("revoke/{userId:int}")]
    public async Task<IActionResult> RevokeAccessFeatures(int userId, [FromBody] int[] accessFeatureIds)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await userService.RevokeAccessFeaturesAsync(userId, accessFeatureIds);
        throw ResponseFactory.Create<OkResponse>(["Access feature was revoked from user successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet]
    public async Task<IActionResult> GetAccessFeatures()
    {
        IEnumerable<AccessFeature> accessFeatures = (await accessFeatureService.GetAllAsync()).ToList();
        IEnumerable<AccessFeatureDTO> dtos = accessFeatures.Select(af => new AccessFeatureDTO
        {
            Id = af.Id,
            Name = af.Name
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<AccessFeatureDTO>>>(dtos, ["Access features retrieved successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccessFeatureById(int id)
    {
        AccessFeature accessFeature = await accessFeatureService.GetByIdValidatedAsync(id);
        AccessFeatureDTO dto = new AccessFeatureDTO
        {
            Id = accessFeature.Id,
            Name = accessFeature.Name
        };
        throw ResponseFactory.Create<OkResponse<AccessFeatureDTO>>(dto, ["Access feature retrieved successfully"]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetAccessFeaturesByUserId(int userId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        ICollection<AccessFeature> features = await accessFeatureService.GetUserFeaturesByIdAsync(userId);
        ICollection<AccessFeatureDTO> dtos = features.Select(af => new AccessFeatureDTO
        {
            Id = af.Id,
            Name = af.Name
        }).ToList();
        throw ResponseFactory.Create<OkResponse<ICollection<AccessFeatureDTO>>>(dtos, ["User access features were retrieved successfully"]);
    }
}
