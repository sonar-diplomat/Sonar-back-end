using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.UserCore;

public class UserFriendRequest : BaseModel
{
    [Required] public int FromUserId { get; set; }

    [Required] public int ToUserId { get; set; }

    [Required] public DateTime RequestedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public bool? IsAccepted { get; set; }

    [JsonIgnore]
    [ForeignKey("FromUserId")]
    public virtual User FromUser { get; set; }

    [JsonIgnore] [ForeignKey("ToUserId")] public virtual User ToUser { get; set; }
}