namespace Entities.Enums;

public struct MailGunTemplates
{
    public const string twoFA = "2fa";
    public const string confirmEmail = "confirm-email";
    public const string passwordRecovery = "recovery-password";

    public static bool IsValidTemplate(string template)
    {
        return template == twoFA || template == confirmEmail || template == passwordRecovery;
    }
}