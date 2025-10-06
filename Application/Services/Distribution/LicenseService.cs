using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class LicenseService(ILicenseRepository repository) : GenericService<License>(repository), ILicenseService
{
}