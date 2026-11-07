namespace Sonar.Hubs;

internal static class ChatGroups
{
    // A SignalR group is only a subscription, never an authorization grant.
    public static string ForMember(int chatId, int userId) => $"chat:{chatId}:user:{userId}";
}
