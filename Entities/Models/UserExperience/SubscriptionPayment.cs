using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("SubscriptionPayment")]
    public class SubscriptionPayment
    {
        [Key]
        public int Id { get; set; }
        [Required, Column(TypeName = "numeric(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public int BuyerId { get; set; }
        [Required]
        public int SubscriptionPackId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("BuyerId")]
        public virtual User Buyer { get; set; }
        [ForeignKey("SubscriptionPackId")]
        public virtual SubscriptionPack SubscriptionPack { get; set; }
    }
}