using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client;

public class UserPrivacySettingsRepository(SonarContext dbContext, IUserPrivacyGroupService userPrivacyGroupService)
    : GenericRepository<UserPrivacySettings>(dbContext), IUserPrivacySettingsRepository
{
}
