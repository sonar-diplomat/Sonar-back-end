using Entities.Models.Access;
using Entities.Models.Chat;
using Entities.Models.ClientSettings;
using Entities.Models.Distribution;
using Entities.Models.File;
using Entities.Models.Music;
using Entities.Models.UserExperience;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using LibraryModel = Entities.Models.Library.Library;

namespace Entities.Models.UserCore;

[Table("User")]
public class User : IdentityUser<int>
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }


    [MaxLength(16)]
    [MinLength(4)]
    //[JsonIgnore]
    [Required]
    public string Login { get; set; }

    [MaxLength(4000)]
    public string? Biography { get; set; } // md 

    [Required]
    public string PublicIdentifier { get; set; }

    [Required]
    public int AvailableCurrency { get; set; }

    [Required]
    public DateTime RegistrationDate { get; set; }

    [Required]
    public bool Enabled2FA { get; set; }

    // Authenticator apps
    [MaxLength(500)]
    public string? GoogleAuthorizationKey { get; set; }
    // facebook .... and ect...


    [Required]
    public int AvatarImageId { get; set; }

    [Required]
    public int VisibilityStateId { get; set; }

    public int? SubscriptionPackId { get; set; }

    [Required]
    public int UserStateId { get; set; }

    [Required]
    public int SettingsId { get; set; }

    [Required]
    public int InventoryId { get; set; }

    [Required]
    public int LibraryId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("VisibilityStateId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual VisibilityState VisibilityState { get; set; }

    [ForeignKey("LibraryId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual LibraryModel Library { get; set; }

    [ForeignKey("AvatarImageId")]
    public virtual ImageFile AvatarImage { get; set; }

    [ForeignKey("SubscriptionPackId")]
    public virtual SubscriptionPack? SubscriptionPack { get; set; }

    [ForeignKey("UserStateId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual UserState UserState { get; set; }

    [ForeignKey("SettingsId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Settings Settings { get; set; }

    [ForeignKey("InventoryId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Inventory Inventory { get; set; }

    public virtual Artist Artist { get; set; }
    public virtual ICollection<UserSession> UserSessions { get; set; }
    public virtual ICollection<Achievement> AchievementProgresses { get; set; }
    public virtual ICollection<AccessFeature> AccessFeatures { get; set; }

    public virtual ICollection<Message> Messages { get; set; }

    public virtual ICollection<Chat.Chat> Chats { get; set; }

    public virtual ICollection<Collection> Collections { get; set; }

    public virtual ICollection<License> Licenses { get; set; }

    public virtual ICollection<Track> Tracks { get; set; }
    public virtual ICollection<Settings> SettingsBlockedUsers { get; set; }

    public virtual ICollection<ArtistRegistrationRequest> ArtistRegistrationRequests { get; set; }
}