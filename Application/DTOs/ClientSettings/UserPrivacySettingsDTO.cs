namespace Application.DTOs.ClientSettings;

public class UserPrivacySettingsDTO
{
    public int Id { get; set; }
    public UserPrivacyGroupDTO WhichCanViewProfile { get; set; }
    public UserPrivacyGroupDTO WhichCanMessage { get; set; }
}

