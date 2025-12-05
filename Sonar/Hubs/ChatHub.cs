using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Enums;
using Entities.Models.UserCore;
using Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Hubs;

[Authorize]
public class ChatHub(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager,
    IChatService chatService
) : Hub
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
        User user = await CheckAccessFeatures([AccessFeatureStruct.SendMessage]);
        await Clients.OthersInGroup(ChatGroup(chatId)).SendAsync("Typing", new
        {
            ChatId = chatId,
            UserId = user.Id
        });
    }

    public async Task ReadMessages(int chatId, IEnumerable<int> messageIds)
    {
        try
        {
            User user = await GetUserByJwt();
            
            await Logger.AddLog(
                $"ReadMessages called: chatId={chatId}, userId={user.Id}, messageIdsCount={messageIds?.Count() ?? 0}",
                LogCategory.Service,
                LogLevel.Debug
            );

            await chatService.ReadMessagesAsync(chatId, user.Id, messageIds);
            
            await Logger.AddLog(
                $"ReadMessages completed: chatId={chatId}, userId={user.Id}",
                LogCategory.Service,
                LogLevel.Debug
            );
        }
        catch (UnauthorizedResponse ex)
        {
            await Logger.AddLog(
                $"ReadMessages unauthorized: chatId={chatId}, error={ex.Message}",
                LogCategory.Service,
                LogLevel.Warning,
                exception: ex
            );
            throw new HubException("User not authenticated");
        }
        catch (ForbiddenResponse ex)
        {
            await Logger.AddLog(
                $"ReadMessages forbidden: chatId={chatId}, error={ex.Message}",
                LogCategory.Service,
                LogLevel.Warning,
                exception: ex
            );
            throw new HubException("Access denied to chat or messages");
        }
        catch (NotFoundResponse ex)
        {
            await Logger.AddLog(
                $"ReadMessages not found: chatId={chatId}, error={ex.Message}",
                LogCategory.Service,
                LogLevel.Warning,
                exception: ex
            );
            throw new HubException("Chat or messages not found");
        }
        catch (Exception ex)
        {
            await Logger.AddLog(
                $"ReadMessages error: chatId={chatId}, error={ex.Message}, stackTrace={ex.StackTrace}",
                LogCategory.Service,
                LogLevel.Error,
                exception: ex
            );
            throw new HubException($"An error occurred while marking messages as read: {ex.Message}");
        }
    }

    public async Task ReadAllMessages(int chatId)
    {
        try
        {
            User user = await GetUserByJwt();
            
            await Logger.AddLog(
                $"ReadAllMessages called: chatId={chatId}, userId={user.Id}",
                LogCategory.Service,
                LogLevel.Debug
            );

            await chatService.ReadAllMessagesAsync(user.Id, chatId);
            
            await Logger.AddLog(
                $"ReadAllMessages completed: chatId={chatId}, userId={user.Id}",
                LogCategory.Service,
                LogLevel.Debug
            );
        }
        catch (UnauthorizedResponse ex)
        {
            await Logger.AddLog(
                $"ReadAllMessages unauthorized: chatId={chatId}, error={ex.Message}",
                LogCategory.Service,
                LogLevel.Warning,
                exception: ex
            );
            throw new HubException("User not authenticated");
        }
        catch (ForbiddenResponse ex)
        {
            await Logger.AddLog(
                $"ReadAllMessages forbidden: chatId={chatId}, error={ex.Message}",
                LogCategory.Service,
                LogLevel.Warning,
                exception: ex
            );
            throw new HubException("Access denied to chat");
        }
        catch (NotFoundResponse ex)
        {
            await Logger.AddLog(
                $"ReadAllMessages not found: chatId={chatId}, error={ex.Message}",
                LogCategory.Service,
                LogLevel.Warning,
                exception: ex
            );
            throw new HubException("Chat not found");
        }
        catch (Exception ex)
        {
            await Logger.AddLog(
                $"ReadAllMessages error: chatId={chatId}, error={ex.Message}, stackTrace={ex.StackTrace}",
                LogCategory.Service,
                LogLevel.Error,
                exception: ex
            );
            throw new HubException($"An error occurred while marking all messages as read: {ex.Message}");
        }
    }

    private static string ChatGroup(int chatId)
    {
        return $"chat:{chatId}";
    }

    [Authorize]
    protected async Task<User> GetUserByJwt()
    {
        HttpContext? httpContext = Context.GetHttpContext() ?? httpContextAccessor.HttpContext;
        ClaimsPrincipal? principal = Context.User ?? httpContext?.User;
        
        if (principal == null)
        {
            await Logger.AddLog(
                $"GetUserByJwt: No user principal found. Context.User is null, HttpContext is {(httpContext == null ? "null" : "not null")}",
                LogCategory.Service,
                LogLevel.Warning
            );
            throw ResponseFactory.Create<UnauthorizedResponse>();
        }
        
        string? userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            await Logger.AddLog(
                $"GetUserByJwt: Invalid or missing userId claim. Claims: {string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}"))}",
                LogCategory.Service,
                LogLevel.Warning
            );
            throw ResponseFactory.Create<UnauthorizedResponse>();
        }

        User? user = await userManager.Users
            .Include(u => u.AccessFeatures)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
        {
            await Logger.AddLog(
                $"GetUserByJwt: User not found in database. userId={userId}",
                LogCategory.Service,
                LogLevel.Warning
            );
            throw ResponseFactory.Create<UnauthorizedResponse>();
        }
        
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