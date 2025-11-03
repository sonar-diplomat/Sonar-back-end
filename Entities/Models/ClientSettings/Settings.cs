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

    [ForeignKey("PreferredPlaybackQualityId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public PlaybackQuality PreferredPlaybackQuality { get; set; }

    [ForeignKey("LanguageId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Language Language { get; set; }

    [ForeignKey("ThemeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Theme Theme { get; set; }

    [ForeignKey("UserPrivacySettingsId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public UserPrivacySettings UserPrivacy { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<NotificationType> NotificationTypes { get; set; }
    public virtual ICollection<User> BlockedUsers { get; set; }
}
