using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    [JsonIgnore]
    [ForeignKey("MessageId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Message Message { get; set; }

    [JsonIgnore]
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual User User { get; set; }
}
