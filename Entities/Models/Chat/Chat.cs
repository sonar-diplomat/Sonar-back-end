using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models;

[Table("Chat")]
public class Chat : BaseModel
{
    [Required, MaxLength(30)]
    public string Name { get; set; }
    [Required]
    public bool IsGroup { get; set; }
        
    [Required]
    public int CoverId { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("CoverId")]
    public virtual File Cover { get; set; } // type: Image

    public virtual ICollection<User> Users { get; set; }
}