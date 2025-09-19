using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("Artist")]
public class Artist
{
    [Key]
    public int Id { get; set; }
    
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