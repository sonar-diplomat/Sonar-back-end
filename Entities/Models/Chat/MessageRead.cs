using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    /// 
    /// </summary>
    [ForeignKey("MessageId")]
    public virtual Message Message { get; set; }
    [ForeignKey("UserId")]
    public virtual UserCore.User User { get; set; }
}