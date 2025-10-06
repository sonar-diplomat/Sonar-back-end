using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Chat;

[Table("MessageRead")]
public class MessageRead : BaseModel
{
    public DateTime? ReadAt { get; set; }

    [Required]
    public int MessageId { get; set; }

    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("MessageId")]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual Message Message { get; set; }

    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User User { get; set; }
}