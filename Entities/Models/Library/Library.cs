using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;

namespace Entities.Models.Library;

[Table("Library")]
public class Library : BaseModel
{
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}