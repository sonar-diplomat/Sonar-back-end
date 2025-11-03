using Application.DTOs.Access;

namespace Application.DTOs.User;

public class UserAdminDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string PublicIdentifier { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool Enabled2FA { get; set; }
    public int AvailableCurrency { get; set; }
    
    // ID properties
    public int AvatarImageId { get; set; }
    public int VisibilityStateId { get; set; }
    public int? SubscriptionPackId { get; set; }
    public int UserStateId { get; set; }
    public int SettingsId { get; set; }
    public int InventoryId { get; set; }
    public int LibraryId { get; set; }
    
    // Related data
    public List<AccessFeatureDTO> AccessFeatures { get; set; }
}

