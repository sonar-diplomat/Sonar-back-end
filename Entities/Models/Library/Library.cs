using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Library;

[Table("Library")]
public class Library : BaseModel
{
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("UserId")]
    public virtual UserCore.User User { get; set; }
}