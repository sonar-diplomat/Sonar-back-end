using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Library;

[Table("Library")]
public class Library : BaseModel
{
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual User User { get; set; }
}