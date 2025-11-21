using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Access;
using Application.Response;
using Entities.Enums;
using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessFeatureController(UserManager<User> userManager, IAccessFeatureService accessFeatureService, IUserService userService) : BaseController(userManager)
{
    /// <summary>
    /// Assigns access features (permissions) to a user.
    /// </summary>
    /// <param name="userId">The ID of the user to assign features to.</param>
    /// <param name="accessFeatureIds">Array of access feature IDs to assign.</param>
    /// <returns>Success response upon assignment.</returns>
    /// <response code="200">Access features assigned successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageUsers' feature).</response>
    /// <response code="404">User or access feature not found.</response>
    [HttpPost("assign/{userId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignAccessFeatures(int userId, [FromBody] int[] accessFeatureIds)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await userService.AssignAccessFeaturesAsync(userId, accessFeatureIds);
        throw ResponseFactory.Create<OkResponse>(["Access feature was assigned to user successfully"]);
    }

    /// <summary>
    /// Revokes access features (permissions) from a user.
    /// </summary>
    /// <param name="userId">The ID of the user to revoke features from.</param>
    /// <param name="accessFeatureIds">Array of access feature IDs to revoke.</param>
    /// <returns>Success response upon revocation.</returns>
    /// <response code="200">Access features revoked successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageUsers' feature).</response>
    [HttpPost("revoke/{userId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeAccessFeatures(int userId, [FromBody] int[] accessFeatureIds)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await userService.RevokeAccessFeaturesAsync(userId, accessFeatureIds);
        throw ResponseFactory.Create<OkResponse>(["Access feature was revoked from user successfully"]);
    }

    /// <summary>
    /// Retrieves all available access features in the system.
    /// </summary>
    /// <returns>List of access feature DTOs.</returns>
    /// <response code="200">Access features retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<AccessFeatureDTO>>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Retrieves a specific access feature by its ID.
    /// </summary>
    /// <param name="id">The ID of the access feature to retrieve.</param>
    /// <returns>Access feature DTO.</returns>
    /// <response code="200">Access feature retrieved successfully.</response>
    /// <response code="404">Access feature not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OkResponse<AccessFeatureDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccessFeatureById(int id)
    {
        AccessFeature accessFeature = await accessFeatureService.GetByIdValidatedAsync(id);
        AccessFeatureDTO dto = new()
        {
            Id = accessFeature.Id,
            Name = accessFeature.Name
        };
        throw ResponseFactory.Create<OkResponse<AccessFeatureDTO>>(dto, ["Access feature retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves all access features assigned to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve features for.</param>
    /// <returns>Collection of access feature DTOs assigned to the user.</returns>
    /// <response code="200">User access features retrieved successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageUsers' feature).</response>
    /// <response code="404">User not found.</response>
    [HttpGet("user/{userId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<ICollection<AccessFeatureDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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
