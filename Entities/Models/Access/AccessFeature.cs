using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models;

[Table("AccessFeature")]
public class AccessFeature : BaseModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<User> Users { get; set; }
}