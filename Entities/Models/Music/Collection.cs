using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Access;
using Infrastructure;

namespace Entities.Models.Music;

public abstract class Collection : BaseModel
{
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
    public virtual File.File Cover { get; set; }
    
    public virtual ICollection<UserCore.User> Users { get; set; }
    public virtual ICollection<Track> Tracks { get; set; }
}