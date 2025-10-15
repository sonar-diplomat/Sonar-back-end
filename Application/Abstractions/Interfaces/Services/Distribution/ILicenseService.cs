using Application.DTOs;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services;

public interface ILicenseService : IGenericService<License>
{
    Task<License> CreateLicenseAsync(DateTime expirationDate, int issuerId);
    Task<License?> GetLicenseByKeyAsync(string licenseKey);
    Task ExtendLicenseAsync(int id, DateTime extendBy);
    Task CloseLicenseAsync(int id);
}
