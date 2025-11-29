using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Application.DTOs.Distribution;
using Application.Response;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sonar.Extensions;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class DistributorController(
    UserManager<User> userManager,
    IDistributorService distributorService,
    ILicenseService licenseService,
    IDistributorAccountService distributorAccountService,
    IImageFileService imageFileService
) : BaseControllerExtended(userManager, distributorAccountService, distributorService)
{
    private readonly IDistributorService distributorService = distributorService;

    /// <summary>
    /// Creates a new distributor in the system.
    /// </summary>
    /// <param name="dto">Distributor creation data including name, cover image, and license expiration date.</param>
    /// <returns>Distributor DTO with created distributor details and license information.</returns>
    /// <response code="200">Distributor created successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageDistributors' feature).</response>
    /// <response code="400">Invalid distributor data or image file.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<DistributorDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDistributor([FromForm] CreateDistributorDTO dto)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        int coverId = (await imageFileService.UploadFileAsync(dto.Cover)).Id;
        License licence = await licenseService.CreateLicenseAsync(dto.ExpirationDate, user.Id);
        int licenceId = licence.Id;
        Distributor distributor = await distributorService.CreateDistributorAsync(dto, licenceId, coverId);
        DistributorDTO responseDto = new()
        {
            Id = distributor.Id,
            Name = distributor.Name,
            CoverId = distributor.CoverId,
            License = new LicenseDTO
            {
                Id = licenceId,
                IssuingDate = DateTime.UtcNow,
                ExpirationDate = licence.ExpirationDate,
                IssuerId = user.Id
            }
        };
        throw ResponseFactory.Create<OkResponse<DistributorDTO>>(responseDto, ["Distributors created successfully"]);
    }

    /// <summary>
    /// Retrieves all distributors in the system.
    /// </summary>
    /// <returns>List of distributor DTOs with license information.</returns>
    /// <response code="200">Distributors retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<DistributorDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDistributors()
    {
        IEnumerable<Distributor> distributors = (await distributorService.GetAllAsync()).ToList();
        IEnumerable<DistributorDTO> dtos = distributors.Select(d => new DistributorDTO
        {
            Id = d.Id,
            Name = d.Name,
            CoverId = d.CoverId,
            License = new LicenseDTO
            {
                Id = d.License.Id,
                IssuingDate = DateTime.UtcNow,
                ExpirationDate = d.License.ExpirationDate,
                IssuerId = d.License.IssuerId,
            }
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<DistributorDTO>>>(dtos,
            ["Distributors retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves detailed information about a specific distributor.
    /// </summary>
    /// <param name="id">The ID of the distributor to retrieve.</param>
    /// <returns>Distributor DTO with full details and license information.</returns>
    /// <response code="200">Distributor retrieved successfully.</response>
    /// <response code="404">Distributor not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OkResponse<DistributorDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDistributorById(int id)
    {
        Distributor d = await distributorService.GetByIdValidatedAsync(id);
        DistributorDTO dto = new()
        {
            Id = d.Id,
            Name = d.Name,
            CoverId = d.CoverId,
            License = new LicenseDTO
            {
                Id = d.License.Id,
                IssuingDate = DateTime.UtcNow,
                ExpirationDate = d.License.ExpirationDate,
                IssuerId = d.License.IssuerId,
            }
        };
        throw ResponseFactory.Create<OkResponse<DistributorDTO>>(dto, ["Distributor retrieved successfully"]);
    }

    /// <summary>
    /// Updates an existing distributor's information.
    /// </summary>
    /// <param name="id">The ID of the distributor to update.</param>
    /// <param name="dto">Updated distributor data.</param>
    /// <returns>Updated distributor DTO.</returns>
    /// <response code="200">Distributor updated successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageDistributors' feature).</response>
    /// <response code="404">Distributor not found.</response>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<DistributorDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDistributor(int id, UpdateDistributorDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor d = await distributorService.UpdateDistributorAsync(id, dto);
        DistributorDTO responseDto = new()
        {
            Id = d.Id,
            Name = d.Name,
            CoverId = d.CoverId,
            License = new LicenseDTO
            {
                Id = d.License.Id,
                IssuingDate = DateTime.UtcNow,
                ExpirationDate = d.License.ExpirationDate,
                IssuerId = d.License.IssuerId,
            }
        };
        throw ResponseFactory.Create<OkResponse<DistributorDTO>>(responseDto, ["Distributor updated successfully"]);
    }

    /// <summary>
    /// Deletes a distributor from the system.
    /// </summary>
    /// <param name="id">The ID of the distributor to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Distributor deleted successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageDistributors' feature).</response>
    /// <response code="404">Distributor not found.</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDistributor(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor distributor = await distributorService.GetByIdValidatedAsync(id);
        await distributorService.DeleteAsync(distributor);
        throw ResponseFactory.Create<OkResponse>(["Distributor deleted successfully"]);
    }

    /// <summary>
    /// Regenerates the API key for a distributor's license.
    /// </summary>
    /// <param name="id">The license ID for which to update the API key.</param>
    /// <returns>The new API key.</returns>
    /// <response code="200">API key updated successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageDistributors' feature).</response>
    /// <response code="404">License not found.</response>
    [HttpGet("update-key/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLicenseKey(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        string key = await licenseService.UpdateLicenseKeyAsync(id);
        throw ResponseFactory.Create<OkResponse<string>>(key, ["Api key updated successfully"]);
    }

    /// <summary>
    /// Retrieves all artist registration requests for the authenticated distributor.
    /// </summary>
    /// <returns>List of artist registration request DTOs.</returns>
    /// <response code="200">Registration requests retrieved successfully.</response>
    /// <response code="401">User not authenticated or not a distributor.</response>
    [HttpGet("request")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ArtistRegistrationRequestDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetArtistRequest()
    {
        int distributorId = (await this.GetDistributorAccountByJwtAsync()).DistributorId;
        IEnumerable<ArtistRegistrationRequest> requests = await distributorService.GetAllRegistrationRequestsAsync(distributorId);
        IEnumerable<ArtistRegistrationRequestDTO> dtos = requests.Select(r => new ArtistRegistrationRequestDTO
        {
            UserId = r.Id,
            ArtistName = r.ArtistName,
            RequestedAt = r.RequestedAt,
            ResolvedAt = r.ResolvedAt,
            DistributorId = r.DistributorId
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<ArtistRegistrationRequestDTO>>>(dtos,
            ["Artist registration requests retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves a specific artist registration request by ID.
    /// </summary>
    /// <param name="requestId">The ID of the registration request to retrieve.</param>
    /// <returns>Artist registration request DTO with full details.</returns>
    /// <response code="200">Registration request retrieved successfully.</response>
    /// <response code="401">User not authenticated or not a distributor.</response>
    /// <response code="404">Request not found or not associated with this distributor.</response>
    [HttpGet("request/{requestId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<ArtistRegistrationRequestDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArtistRequestById(int requestId)
    {
        int distributorId = (await this.GetDistributorAccountByJwtAsync()).DistributorId;
        ArtistRegistrationRequest request = await distributorService.GetRegistrationRequestByIdAsync(distributorId, requestId);
        ArtistRegistrationRequestDTO dto = new()
        {
            Id = requestId,
            UserId = request.UserId,
            ArtistName = request.ArtistName,
            RequestedAt = request.RequestedAt,
            ResolvedAt = request.ResolvedAt,
            DistributorId = request.DistributorId
        };
        throw ResponseFactory.Create<OkResponse<ArtistRegistrationRequestDTO>>(dto, ["Artist registration request retrieved successfully"]);
    }

    /// <summary>
    /// Resolves an artist registration request by approving or rejecting it.
    /// </summary>
    /// <param name="requestId">The ID of the registration request to resolve.</param>
    /// <param name="approve">True to approve the request, false to reject it.</param>
    /// <returns>Success response upon resolution.</returns>
    /// <response code="200">Registration request resolved successfully.</response>
    /// <response code="401">User not authenticated or not a distributor.</response>
    /// <response code="404">Request not found.</response>
    [HttpPost("request/{requestId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveArtistRequest(int requestId, [FromQuery] bool approve)
    {
        int distributorId = (await this.GetDistributorAccountByJwtAsync()).DistributorId;
        await distributorService.ResolveArtistRequestAsync(distributorId, requestId, approve);
        throw ResponseFactory.Create<OkResponse>(["Artist registration request resolved successfully"]);
    }
}