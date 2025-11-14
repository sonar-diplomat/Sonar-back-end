using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserCore;

[Table("UserState")]
public class UserState : BaseModel
{
    public int? PrimarySessionId { get; set; }

    [Required]
    public int UserStatusId { get; set; }

    [Required]
    public int QueueId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("PrimarySessionId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual UserSession? PrimarySession { get; set; }

    [JsonIgnore]
    [ForeignKey("UserStatusId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual UserStatus UserStatus { get; set; }

    [JsonIgnore]
    [ForeignKey("QueueId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Queue Queue { get; set; }

    [JsonIgnore]
    public virtual User User { get; set; }
}