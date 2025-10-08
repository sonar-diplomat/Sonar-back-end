using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserExperience;

[Table("SubscriptionPayment")]
public class SubscriptionPayment : BaseModel
{
    [Required]
    [Column(TypeName = "numeric(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public int BuyerId { get; set; }

    [Required]
    public int SubscriptionPackId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("BuyerId")]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual User Buyer { get; set; }

    [ForeignKey("SubscriptionPackId")]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual SubscriptionPack SubscriptionPack { get; set; }
}
