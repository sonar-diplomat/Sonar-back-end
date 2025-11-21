namespace Application.DTOs.ClientSettings;

public class SettingsDTO
{
    public int Id { get; set; }
    public bool AutoPlay { get; set; }
    public bool Crossfade { get; set; }
    public bool ExplicitContent { get; set; }
    
    // Related objects
    public PlaybackQualityDTO PreferredPlaybackQuality { get; set; }
    public LanguageDTO Language { get; set; }
    public ThemeDTO Theme { get; set; }
    public UserPrivacySettingsDTO UserPrivacy { get; set; }
    
    // Collections
    public List<NotificationTypeDTO> NotificationTypes { get; set; }
    public List<int> BlockedUserIds { get; set; }
}

