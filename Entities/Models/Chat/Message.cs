using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Chat;

[Table("Message")]
public class Message : BaseModel
{
    [Required]
    [MaxLength(2000)]
    public string TextContent { get; set; }

    public int? ReplyMessageId { get; set; }

    [Required]
    public int ChatId { get; set; }

    [Required]
    public int SenderId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("ReplyMessageId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Message? ReplyMessage { get; set; }

    [ForeignKey("ChatId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Chat Chat { get; set; }

    [ForeignKey("SenderId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Sender { get; set; }

    public virtual ICollection<MessageRead> MessagesReads { get; set; }
}