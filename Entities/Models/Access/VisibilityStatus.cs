using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models;

[Table("VisibilityStatus")]
public class VisibilityStatus : BaseModel
{
    [Required, MaxLength(30)]
    public string Name { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<VisibilityState> VisibilityStates { get; set; }
}