using System.Security.Claims;
using Application.Response;
using Entities.Enums;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Hubs;

[Authorize]
public class ChatHub(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) : Hub
{
    public async Task JoinChat(int chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(chatId));
    }

    public async Task LeaveChat(int chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroup(chatId));
    }

    public async Task Typing(int chatId)
    {
        await Clients.OthersInGroup(ChatGroup(chatId)).SendAsync("Typing", new
        {
            ChatId = chatId,
            UserId = CheckAccessFeatures([AccessFeatureStruct.SendMessage])
        });
    }

    private static string ChatGroup(int chatId)
    {
        return $"chat:{chatId}";
    }

    [Authorize]
    protected async Task<User> GetUserByJwt()
    {
        HttpContext httpContext = httpContextAccessor.HttpContext;
        ClaimsPrincipal principal = httpContext?.User ?? throw ResponseFactory.Create<UnauthorizedResponse>();
        User? user = await userManager.GetUserAsync(principal);
        user = user == null
            ? throw ResponseFactory.Create<UnauthorizedResponse>()
            : userManager.Users.Include(u => u.AccessFeatures).FirstOrDefault(u => u.Id == user.Id);
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
        return user.AccessFeatures.All(af => !features.Contains(af.Name))
            ? throw ResponseFactory.Create<ForbiddenResponse>(["You do not have permission to perform this action"])
            : user;
    }
}