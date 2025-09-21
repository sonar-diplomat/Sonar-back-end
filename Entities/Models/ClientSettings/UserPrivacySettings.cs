using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models
{
    [Table("UserPrivacySettings")]
    public class UserPrivacySettings : BaseModel
    {
        [Required]
        public int SettingsId { get; set; }
        [Required]
        public int WhichCanViewProfileId { get; set; }
        [Required]
        public int WhichCanMessageId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("SettingsId")]
        public virtual Settings Settings { get; set; }
        [ForeignKey("WhichCanViewProfileId")]
        public virtual UserPrivacyGroup WhichCanViewProfile { get; set; }
        [ForeignKey("WhichCanMessageId")]
        public virtual UserPrivacyGroup WhichCanMessage { get; set; }
    }
}