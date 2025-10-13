using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserExperience;

[Table("Gift")]
public class Gift : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string TextContent { get; set; } // markdown

    [Required]
    public DateTime GiftTime { get; set; }

    [Required]
    public DateTime AcceptanceDate { get; set; }

    [Required]
    public int GiftStyleId { get; set; }

    [Required]
    public int ReceiverId { get; set; }

    [Required]
    public int SubscriptionPaymentId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("ReceiverId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Receiver { get; set; }

    [ForeignKey("GiftStyleId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual GiftStyle GiftStyle { get; set; }

    [ForeignKey("SubscriptionPaymentId")]
    public virtual SubscriptionPayment SubscriptionPayment { get; set; }
}
