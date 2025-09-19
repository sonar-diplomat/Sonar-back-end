using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("VisibilityStatus")]
public class VisibilityStatus
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(30)]
    public string Name { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<VisibilityState> VisibilityStates { get; set; }
}