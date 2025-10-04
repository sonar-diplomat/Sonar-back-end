using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserCore
{
    [Table("UserState")]
    public class UserState : BaseModel
    {

        public int? PrimarySessionId { get; set; }
        [Required]
        public int UserStatusId { get; set; }
        public int? QueueId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("PrimarySessionId")]
        public virtual UserSession PrimarySession { get; set; }
        [ForeignKey("UserStatusId")]
        public virtual UserStatus UserStatus { get; set; }
        [ForeignKey("QueueId")]
        public virtual Queue? Queue { get; set; }
        public virtual User User { get; set; }


    }
}
