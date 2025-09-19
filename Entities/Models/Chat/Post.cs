using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("Post")]
public class Post
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Title { get; set; }
    [Required, MaxLength(5000)]
    public string TextContent { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public int UserId { get; set; }
    [Required]
    public int VisibilityStateId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    [ForeignKey("VisibilityStateId")]
    public virtual VisibilityState VisibilityState { get; set; }
    
    public virtual ICollection<File> Files { get; set; }
}