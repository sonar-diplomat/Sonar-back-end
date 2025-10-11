namespace Entities.Enums;

public struct AccessFeatureStruct
{
    // --- Social ---
    public const string SendFriendRequest = "SendFriendRequest";
    public const string SendMessage = "SendMessage";
    public const string ReportContent = "ReportContent";

    // --- Content ---
    public const string ListenContent = "ListenContent";

    // --- Access ---
    public const string UserLogin = "UserLogin";

    // --- Moderate ---
    public const string ManageUsers = "ManageUsers";
    public const string ManageContent = "ManageContent";
    public const string ManageDistributors = "ManageDistributors";
    public const string ManageReports = "ManageReports";
    public const string IamAGod = "IamAGod";

    // --- Artists ---
    public const string CreatePost = "CreatePost";
}
