using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Application.Response;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class DistributorController(
    UserManager<User> userManager,
    IDistributorService distributorService,
    ILicenseService licenseService,
    IImageFileService imageFileService
) : BaseController(userManager)
{
    [HttpPost]
    public async Task<IActionResult> CreateDistributor(CreateDistributorDTO dto, IFormFile cover)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        int coverId = (await imageFileService.UploadFileAsync(cover)).Id;
        int licenceId = (await licenseService.CreateLicenseAsync(dto.ExpirationDate, user.Id)).Id;
        Distributor distributor = await distributorService.CreateDistributorAsync(dto, licenceId, coverId);
        throw ResponseFactory.Create<OkResponse<Distributor>>(distributor, ["Distributors created successfully"]);
    }

    [HttpGet]
    public async Task<IActionResult> GetDistributors()
    {
        IEnumerable<Distributor> distributors = await distributorService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<Distributor>>>(distributors,
            ["Distributors retrieved successfully"]);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDistributorById(int id)
    {
        Distributor? distributor = await distributorService.GetByIdAsync(id);
        return distributor == null
            ? throw ResponseFactory.Create<NotFoundResponse>(["Distributor not found"])
            : throw ResponseFactory.Create<OkResponse<Distributor>>(distributor,
                ["Distributor retrieved successfully"]);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDistributor(int id, UpdateDistributorDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor distributor = await distributorService.UpdateDistributorAsync(id, dto);
        throw ResponseFactory.Create<OkResponse<Distributor>>(distributor, ["Distributor updated successfully"]);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDistributor(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor distributor = await distributorService.GetByIdValidatedAsync(id);
        await distributorService.DeleteAsync(distributor);
        throw ResponseFactory.Create<OkResponse>(["Distributor deleted successfully"]);
    }

    [HttpGet("update-key/{id:int}")]
    public async Task<IActionResult> UpdateLicenseKey(int licenseId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        string key = await licenseService.UpdateLicenseKeyAsync(licenseId);
        throw ResponseFactory.Create<OkResponse<string>>(key, ["Api key updated successfully"]);
    }
}