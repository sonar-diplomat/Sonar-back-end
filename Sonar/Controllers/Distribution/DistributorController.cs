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

    [HttpPost]
    [Authorize]
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
            CoverUrl = distributor.Cover?.Url ?? string.Empty,
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDistributors()
    {
        IEnumerable<Distributor> distributors = (await distributorService.GetAllAsync()).ToList();
        IEnumerable<DistributorDTO> dtos = distributors.Select(d => new DistributorDTO
        {
            Id = d.Id,
            Name = d.Name,
            CoverUrl = d.Cover?.Url ?? string.Empty,
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDistributorById(int id)
    {
        Distributor d = await distributorService.GetByIdValidatedAsync(id);
        DistributorDTO dto = new()
        {
            Id = d.Id,
            Name = d.Name,
            CoverUrl = d.Cover?.Url ?? string.Empty,
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

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateDistributor(int id, UpdateDistributorDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor d = await distributorService.UpdateDistributorAsync(id, dto);
        DistributorDTO responseDto = new()
        {
            Id = d.Id,
            Name = d.Name,
            CoverUrl = d.Cover?.Url ?? string.Empty,
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

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteDistributor(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor distributor = await distributorService.GetByIdValidatedAsync(id);
        await distributorService.DeleteAsync(distributor);
        throw ResponseFactory.Create<OkResponse>(["Distributor deleted successfully"]);
    }

    [HttpGet("update-key/{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateLicenseKey(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        string key = await licenseService.UpdateLicenseKeyAsync(id);
        throw ResponseFactory.Create<OkResponse<string>>(key, ["Api key updated successfully"]);
    }

    [HttpGet("request")]
    [Authorize]
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

    [HttpGet("request/{requestId:int}")]
    [Authorize]
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

    [HttpPost("request/{requestId:int}")]
    [Authorize]
    public async Task<IActionResult> ResolveArtistRequest(int requestId, bool approve)
    {
        int distributorId = (await this.GetDistributorAccountByJwtAsync()).DistributorId;
        await distributorService.ResolveArtistRequestAsync(distributorId, requestId, approve);
        throw ResponseFactory.Create<OkResponse>(["Artist registration request resolved successfully"]);
    }
}