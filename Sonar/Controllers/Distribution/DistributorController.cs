using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Application.Response;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        int licenceId = (await licenseService.CreateLicenseAsync(dto.ExpirationDate, user.Id)).Id;
        Distributor distributor = await distributorService.CreateDistributorAsync(dto, licenceId, coverId);
        throw ResponseFactory.Create<OkResponse<Distributor>>(distributor, ["Distributors created successfully"]);
    }

    [HttpGet]
    public async Task<IActionResult> GetDistributors()
    {
        IEnumerable<Distributor> distributors = (await distributorService.GetAllAsync()).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<Distributor>>>(distributors,
            ["Distributors retrieved successfully"]);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDistributorById(int id)
    {
        Distributor distributor = await distributorService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<Distributor>>(distributor, ["Distributor retrieved successfully"]);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateDistributor(int id, UpdateDistributorDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor distributor = await distributorService.UpdateDistributorAsync(id, dto);
        throw ResponseFactory.Create<OkResponse<Distributor>>(distributor, ["Distributor updated successfully"]);
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
        int distributorId = (await GetDistributorAccountByJwt()).DistributorId;

        IEnumerable<ArtistRegistrationRequest> requests =
            await distributorService.GetAllRegistrationRequestsAsync(distributorId);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ArtistRegistrationRequest>>>(requests,
            ["Artist registration requests retrieved successfully"]);
    }

    [HttpGet("request/{requestId:int}")]
    [Authorize]
    public async Task<IActionResult> GetArtistRequestById(int requestId)
    {
        int distributorId = (await GetDistributorAccountByJwt()).DistributorId;

        ArtistRegistrationRequest request =
            await distributorService.GetRegistrationRequestByIdAsync(distributorId, requestId);
        throw ResponseFactory.Create<OkResponse<ArtistRegistrationRequest>>(request, ["Artist registration request retrieved successfully"]);
    }

    [HttpPost("request/{requestId:int}")]
    [Authorize]
    public async Task<IActionResult> ResolveArtistRequest(int requestId, bool approve)
    {
        int distributorId = (await GetDistributorAccountByJwt()).DistributorId;
        await distributorService.ResolveArtistRequestAsync(distributorId, requestId, approve);
        throw ResponseFactory.Create<OkResponse>(["Artist registration request resolved successfully"]);
    }
}