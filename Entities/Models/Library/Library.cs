using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("Library")]
public class Library
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
}