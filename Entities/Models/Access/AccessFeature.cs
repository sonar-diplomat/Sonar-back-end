using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Access;

[Table("AccessFeature")]
public class AccessFeature : BaseModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<UserCore.User> Users { get; set; }
}