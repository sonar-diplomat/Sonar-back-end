using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models.Access;

[Table("VisibilityState")]
public class VisibilityState : BaseModel
{
    [Required] 
    public DateTime SetPublicOn { get; set; }

    [Required] 
    public int StatusId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("StatusId")]
    public virtual VisibilityStatus Status { get; set; }
}