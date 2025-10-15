using Application.Exception;
using Entities.Enums;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

public abstract class BaseController(UserManager<User> userManager) : ControllerBase
{
    [Authorize]
    protected async Task<User> GetUserByJwt()
    {
        User? user = await userManager.GetUserAsync(User);
        return user ?? throw AppExceptionFactory.Create<UnauthorizedException>();
    }

    [Authorize] 
    protected async Task<User> CheckAccessFeatures(string[] feature)
    {
        User user = await GetUserByJwt();
        if (user.AccessFeatures.FirstOrDefault(af => af.Name == AccessFeatureStruct.IamAGod) != null)
            return user;
        
        string[] baseRoles = [AccessFeatureStruct.UserLogin];
        IEnumerable<string> features = baseRoles.Concat(feature);
        return user.AccessFeatures.All(af => !features.Contains(af.Name)) ? throw AppExceptionFactory.Create<ForbiddenException>(["You do not have permission to perform this action"]) : user;
    }
}
