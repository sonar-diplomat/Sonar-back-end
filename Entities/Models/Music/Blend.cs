using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("Blend")]
public class Blend : Collection
{
    public virtual ICollection<User> Users { get; set; }
}