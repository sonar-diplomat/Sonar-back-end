using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("MessageRead")]
public class MessageRead
{
    [Key]
    public int Id { get; set; }
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
    public virtual User User { get; set; }
}