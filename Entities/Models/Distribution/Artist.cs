using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models;

[Table("Artist")]
public class Artist : BaseModel
{
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
    public virtual ICollection<Track> Tracks { get; set; }
    public virtual ICollection<Album> Albums { get; set; }
}