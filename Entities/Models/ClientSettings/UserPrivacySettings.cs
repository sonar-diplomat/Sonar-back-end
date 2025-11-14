using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.ClientSettings;

[Table("UserPrivacySettings")]
public class UserPrivacySettings : BaseModel
{
    [Required]
    public int WhichCanViewProfileId { get; set; }

    [Required]
    public int WhichCanMessageId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("WhichCanViewProfileId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual UserPrivacyGroup WhichCanViewProfile { get; set; }

    [JsonIgnore]
    [ForeignKey("WhichCanMessageId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual UserPrivacyGroup WhichCanMessage { get; set; }
}
