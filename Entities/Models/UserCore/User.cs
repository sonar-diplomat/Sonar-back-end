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
    [Required] [MaxLength(100)] public string FirstName { get; set; }

    [Required] [MaxLength(100)] public string LastName { get; set; }

    [Required] public DateOnly DateOfBirth { get; set; }


    [MaxLength(16)]
    [MinLength(4)]
    //[JsonIgnore]
    [Required]
    public string Login { get; set; }

    [MaxLength(4000)] public string? Biography { get; set; } // md 

    [Required] public string PublicIdentifier { get; set; }

    [Required] public int AvailableCurrency { get; set; }

    [Required] public DateTime RegistrationDate { get; set; }

    [Required] public bool Enabled2FA { get; set; }

    // Authenticator apps
    [MaxLength(500)] public string? GoogleAuthorizationKey { get; set; }
    // facebook .... and ect...


    [Required] public int AvatarImageId { get; set; }

    [Required] public int VisibilityStateId { get; set; }

    public int? SubscriptionPackId { get; set; }

    [Required] public int UserStateId { get; set; }

    [Required] public int SettingsId { get; set; }

    [Required] public int InventoryId { get; set; }

    [Required] public int LibraryId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("VisibilityStateId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual VisibilityState VisibilityState { get; set; }

    [JsonIgnore]
    [ForeignKey("LibraryId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual LibraryModel Library { get; set; }

    [JsonIgnore]
    [ForeignKey("AvatarImageId")]
    public virtual ImageFile AvatarImage { get; set; }

    [JsonIgnore]
    [ForeignKey("SubscriptionPackId")]
    public virtual SubscriptionPack? SubscriptionPack { get; set; }

    [JsonIgnore]
    [ForeignKey("UserStateId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual UserState UserState { get; set; }

    [JsonIgnore]
    [ForeignKey("SettingsId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Settings Settings { get; set; }

    [JsonIgnore]
    [ForeignKey("InventoryId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Inventory Inventory { get; set; }

    [JsonIgnore] public virtual Artist Artist { get; set; }
    [JsonIgnore] public virtual ICollection<UserSession> UserSessions { get; set; }
    [JsonIgnore] public virtual ICollection<Achievement> AchievementProgresses { get; set; }
    [JsonIgnore] public virtual ICollection<AccessFeature> AccessFeatures { get; set; }

    [JsonIgnore] public virtual ICollection<Message> Messages { get; set; }

    [JsonIgnore] public virtual ICollection<Chat.Chat> ChatsWhereMember { get; set; }

    [JsonIgnore] public virtual ICollection<Chat.Chat> ChatsWhereCreator { get; set; }

    [JsonIgnore] public virtual ICollection<Chat.Chat> ChatsWhereAdmin { get; set; }

    [JsonIgnore] public virtual ICollection<Collection> Collections { get; set; }

    [JsonIgnore] public virtual ICollection<License> Licenses { get; set; }

    [JsonIgnore] public virtual ICollection<Track> Tracks { get; set; }
    [JsonIgnore] public virtual ICollection<Settings> SettingsBlockedUsers { get; set; }
    [JsonIgnore] public virtual ICollection<MessageRead> MessagesReads { get; set; }

    [JsonIgnore] public virtual ICollection<ArtistRegistrationRequest> ArtistRegistrationRequests { get; set; }

    [JsonIgnore] public virtual ICollection<UserFriendRequest> SentFriendRequests { get; set; }

    [JsonIgnore] public virtual ICollection<UserFriendRequest> ReceivedFriendRequests { get; set; }

    [JsonIgnore] public virtual ICollection<User> Friends { get; set; }

    [JsonIgnore] public virtual ICollection<User> FriendOf { get; set; }

    [JsonIgnore] public virtual ICollection<UserFollow> Followers { get; set; }

    [JsonIgnore] public virtual ICollection<UserFollow> Following { get; set; }
}