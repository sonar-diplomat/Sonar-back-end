using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public int NotificationTypeId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int UserPrivacySettingsId { get; set; }

    [ForeignKey("PreferredPlaybackQualityId")]
    public PlaybackQuality PreferredPlaybackQuality { get; set; }
    [ForeignKey("LanguageId")]
    public Language Language { get; set; }
    [ForeignKey("ThemeId")]
    public Theme Theme { get; set; }
    [ForeignKey("NotificationTypeId")]
    public NotificationType NotificationType { get; set; }
    [ForeignKey("UserId")]
    public UserCore.User User { get; set; }
    [ForeignKey("UserPrivacySettingsId")]
    public UserPrivacySettings UserPrivacy { get; set; }
}