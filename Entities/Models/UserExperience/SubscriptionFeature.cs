using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserExperience;

[Table("SubscriptionFeature")]
public class SubscriptionFeature : BaseModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; }
    [Required, MaxLength(1000)]
    public string Description { get; set; }
    [Required, Column(TypeName = "numeric(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<SubscriptionPack> SubscriptionPacks { get; set; }
}