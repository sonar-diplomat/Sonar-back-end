using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required, MaxLength(255), EmailAddress]
        public string Email { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(4000)]
        public string Biography { get; set; } // md 
        [Required]
        public string PublicIdentifier { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
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
        public virtual File AvatarImage { get; set; }
        [ForeignKey("SubscriptionPackId")]
        public virtual SubscriptionPack SubscriptionPack { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual UserState UserState { get; set; }
        public virtual Settings Settings { get; set; }
        
        public virtual ICollection<UserSession> UserSessions { get; set; }
        public virtual ICollection<Achievement> AchievementProgresses { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<AccessFeature> AccessFeatures { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
        public virtual ICollection<License> Licenses { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}