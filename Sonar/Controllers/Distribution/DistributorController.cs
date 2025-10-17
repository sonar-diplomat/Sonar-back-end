using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
using Application.Exception;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.File;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class DistributorController(
    UserManager<User> userManager,
    IDistributorService distributorService,
    ILicenseService licenseService,
    IFileService fileService,
    IFileTypeService fileTypeService
    ) : BaseController(userManager)
{
    [HttpPost]
    public async Task<IActionResult> CreateDistributor(CreateDistributorDTO dto ,IFormFile cover)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);

        FileType fileType = await  fileTypeService.GetByNameAsync("image")
                                 ?? throw AppExceptionFactory.Create<NotFoundException>(["File type 'Image' not found"]);
        int coverId = (await fileService.UploadFileAsync(fileType, cover)).Id;
        int licenceId = (await licenseService.CreateLicenseAsync(dto.ExpirationDate, user.Id)).Id;
        Distributor distributor = await distributorService.CreateDistributorAsync(dto, licenceId, coverId);
        return Ok(new BaseResponse<Distributor>(distributor, "Distributors created successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> GetDistributors()
    {
        IEnumerable<Distributor> distributors = await distributorService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<Distributor>>(distributors, "Distributors retrieved successfully"));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDistributorById(int id)
    {
        Distributor? distributor = await distributorService.GetByIdAsync(id);
        return distributor == null ? throw AppExceptionFactory.Create<NotFoundException>(["Distributor not found"]) 
            : Ok(new BaseResponse<Distributor>(distributor, "Distributor retrieved successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDistributor(int id, UpdateDistributorDTO dto)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor distributor = await distributorService.UpdateDistributorAsync(id, dto);
        return Ok(new BaseResponse<Distributor>(distributor, "Distributor updated successfully"));
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDistributor(int id)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ManageDistributors]);
        Distributor distributor = await distributorService.GetByIdValidatedAsync(id);
        await distributorService.DeleteAsync(distributor);
        return Ok(new BaseResponse<Distributor>(distributor, "Distributor deleted successfully"));
    }
    
    
}
