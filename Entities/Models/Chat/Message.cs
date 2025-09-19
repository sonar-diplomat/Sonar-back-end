using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Message")]
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(2000)]
        public string TextContent { get; set; }
        
        public int? ReplyMessageId { get; set; }
        [Required]
        public int ChatId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("ReplyMessageId")]
        public virtual Message? ReplyMessage { get; set; }
        [ForeignKey("ChatId")]
        public virtual Chat Chat { get; set; }
        
        public virtual ICollection<User> users { get; set; }
    }
}
