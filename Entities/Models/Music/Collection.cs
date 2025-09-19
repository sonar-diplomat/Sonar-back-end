using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public abstract class Collection
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    public int VisibilityStateId { get; set; }
    [Required]
    public int CoverId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("VisibilityStateId")]
    public virtual VisibilityState VisibilityState { get; set; }
    [ForeignKey("CoverId")]
    public virtual File Cover { get; set; }
    
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Track> Tracks { get; set; }
}