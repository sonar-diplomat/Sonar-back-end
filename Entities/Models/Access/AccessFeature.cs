using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("AccessFeature")]
public class AccessFeature
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<User> Users { get; set; }
}