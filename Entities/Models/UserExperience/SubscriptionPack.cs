using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserExperience;

[Table("SubscriptionPack")]
public class SubscriptionPack : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    public double DiscountMultiplier { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// </summary>
    public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
}
