using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;

namespace Entities.Models.ClientSettings;

[Table("UserPrivacySettings")]
public class UserPrivacySettings : BaseModel
{
    [Required]
    public int WhichCanViewProfileId { get; set; }
    [Required]
    public int WhichCanMessageId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("WhichCanViewProfileId")]
    public virtual UserPrivacyGroup WhichCanViewProfile { get; set; }
    [ForeignKey("WhichCanMessageId")]
    public virtual UserPrivacyGroup WhichCanMessage { get; set; }
}