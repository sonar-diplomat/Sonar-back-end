using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.File;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Chat;

[Table("Chat")]
public class Chat : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }

    [Required]
    public bool IsGroup { get; set; }

    [Required]
    public int CoverId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("CoverId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual ImageFile Cover { get; set; } // type: Image

    public virtual ICollection<User> Users { get; set; }
}