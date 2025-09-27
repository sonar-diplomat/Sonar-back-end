using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Access;
using Entities.Models.Chat;
using Entities.Models.ClientSettings;
using Entities.Models.Distribution;
using Entities.Models.Music;
using Entities.Models.UserExperience;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models.UserCore;

[Table("User")]
public class User : IdentityUser<int>
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; }
    [Required, MaxLength(100)]
    public string LastName { get; set; }
    [Required]
    public DateOnly DateOfBirth { get; set; }
    [Required, MaxLength(24), MinLength(1)]
    public string Username { get; set; }
    [Required, MaxLength(16), MinLength(4)]
    public string Login { get; set; }
    [MaxLength(4000)]
    public string Biography { get; set; } // md 
    [Required]
    public string PublicIdentifier { get; set; }

    [Required]
    public int AvailableCurrency { get; set; }
    [Required]
    public DateTime RegistrationDate { get; set; }
    [Required]
    public bool Enabled2FA { get; set; }

    // Authenticator apps
    [StringLength(500)]
    public string GoogleAuthorizationKey { get; set; }
    // facebook .... and ect...

    [Required]
    public int AvatarImageId { get; set; }
    [Required]
    public int VisibilityStateId { get; set; }
    public int? SubscriptionPackId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("VisibilityStateId")]
    public virtual VisibilityState VisibilityState { get; set; }
    [ForeignKey("AvatarImageId")]
    public virtual File.File AvatarImage { get; set; }
    [ForeignKey("SubscriptionPackId")]
    public virtual SubscriptionPack? SubscriptionPack { get; set; }
    public virtual Artist Artist { get; set; }
    public virtual Inventory Inventory { get; set; }
    public virtual UserState UserState { get; set; }
    public virtual Settings Settings { get; set; }

    public virtual ICollection<UserSession> UserSessions { get; set; }
    public virtual ICollection<Achievement> AchievementProgresses { get; set; }
    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<AccessFeature> AccessFeatures { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<Chat.Chat> Chats { get; set; }
    public virtual ICollection<Collection> Collections { get; set; }
    public virtual ICollection<License> Licenses { get; set; }
    public virtual ICollection<Track> Tracks { get; set; }
}