using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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


    [NotMapped]
    public decimal Price { get { return SubscriptionFeatures.Count == 0 ? 0 : SubscriptionFeatures.Select(s => s.Price).Sum(); } }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
}
