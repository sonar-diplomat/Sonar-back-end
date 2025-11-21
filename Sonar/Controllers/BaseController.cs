using Application.Response;
using Entities.Enums;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers;

public abstract class BaseController(
    UserManager<User> userManager
) : ControllerBase
{
    [Authorize]
    protected async Task<User> GetUserByJwt()
    {
        User? user = await userManager.GetUserAsync(User);
        user = user == null ?
            throw ResponseFactory.Create<UnauthorizedResponse>()
            :
            userManager.Users.Include(u => u.AccessFeatures).FirstOrDefault(u => u.Id == user.Id);
        return user;
    }

    [Authorize]
    protected async Task<User> CheckAccessFeatures(string[] feature)
    {
        User user = await GetUserByJwt();
        if (user.AccessFeatures.FirstOrDefault(af => af.Name == AccessFeatureStruct.IamAGod) != null)
            return user;

        string[] baseRoles = [AccessFeatureStruct.UserLogin];
        IEnumerable<string> features = baseRoles.Concat(feature);
        return features.All(f => user.AccessFeatures.Any(af => af.Name == f))
            ? user
            : throw ResponseFactory.Create<ForbiddenResponse>(["You do not have permission to perform this action"]);
    }
}