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

    [Required]
    public int CreatorId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("CoverId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual ImageFile Cover { get; set; }

    [ForeignKey("CreatorId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Creator { get; set; }

    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<User> Members { get; set; }

    public virtual ICollection<User> Admins { get; set; }
}