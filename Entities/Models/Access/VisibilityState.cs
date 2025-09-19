using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class VisibilityState
{
    [Key]
    public int Id { get; set; }
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