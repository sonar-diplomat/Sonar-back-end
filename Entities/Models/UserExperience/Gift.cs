using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Gift")]
    public class Gift
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string TextContent { get; set; } // markdown
        [Required]
        public DateTime GiftTime { get; set; }

        [Required]
        public int GiftStyleId { get; set; }
        [Required]
        public int ReceiverId { get; set; }
        [Required]
        public int SubscriptionPaymentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }
        [ForeignKey("GiftStyleId")]
        public virtual GiftStyle GiftStyle { get; set; }
        [ForeignKey("SubscriptionPaymentId")]
        public virtual SubscriptionPayment SubscriptionPayment { get; set; }

    }

}
