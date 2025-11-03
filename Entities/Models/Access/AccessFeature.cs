using Entities.Models.UserCore;
using NSwag.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Access;

[Table("AccessFeature")]
public class AccessFeature : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// </summary>
    [OpenApiIgnore]
    public virtual ICollection<User> Users { get; set; }
}
