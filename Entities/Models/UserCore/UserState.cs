using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserCore;

[Table("UserState")]
public class UserState : BaseModel
{
    public int? PrimarySessionId { get; set; }

    [Required]
    public int UserStatusId { get; set; }

    public int? QueueId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("PrimarySessionId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual UserSession PrimarySession { get; set; }

    [ForeignKey("UserStatusId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual UserStatus UserStatus { get; set; }

    [ForeignKey("QueueId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Queue? Queue { get; set; }

    public virtual User User { get; set; }
}