using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    /// <summary>
    /// </summary>
    [ForeignKey("ReplyMessageId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Message? ReplyMessage { get; set; }

    [ForeignKey("ChatId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Chat Chat { get; set; }

    public virtual ICollection<User> users { get; set; }
}