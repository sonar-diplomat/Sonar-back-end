using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.ClientSettings;

[Table("Settings")]
public class Settings : BaseModel
{
    [Required]
    public bool AutoPlay { get; set; }

    [Required]
    public bool Crossfade { get; set; }

    [Required]
    public bool ExplicitContent { get; set; }

    [Required]
    public int PreferredPlaybackQualityId { get; set; }

    [Required]
    public int LanguageId { get; set; }

    [Required]
    public int ThemeId { get; set; }

    [Required]
    public int UserPrivacySettingsId { get; set; }

    [JsonIgnore]
    [ForeignKey("PreferredPlaybackQualityId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public PlaybackQuality PreferredPlaybackQuality { get; set; }

    [JsonIgnore]
    [ForeignKey("LanguageId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Language Language { get; set; }

    [JsonIgnore]
    [ForeignKey("ThemeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Theme Theme { get; set; }

    [JsonIgnore]
    [ForeignKey("UserPrivacySettingsId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public UserPrivacySettings UserPrivacy { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
    [JsonIgnore]
    public virtual ICollection<NotificationType> NotificationTypes { get; set; }
    [JsonIgnore]
    public virtual ICollection<User> BlockedUsers { get; set; }
}
