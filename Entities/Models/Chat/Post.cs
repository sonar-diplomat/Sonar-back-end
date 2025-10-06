using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Chat;

[Table("Post")]
public class Post : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    [MaxLength(5000)]
    public string TextContent { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int VisibilityStateId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public User User { get; set; }

    [ForeignKey("VisibilityStateId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public VisibilityState VisibilityState { get; set; }

    public ICollection<File.File> Files { get; set; }
}