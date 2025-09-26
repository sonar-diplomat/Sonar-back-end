using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Music;
using Infrastructure;

namespace Entities.Models.UserCore
{
    [Table("UserState")]
    public class UserState : BaseModel
    {
        public int Track { get; set; }
        public int Position { get; set; }
        
        [Required]
        public int UserId { get; set; }
        [Required]
        public int PrimarySessionId { get; set; }
        [Required]
        public int UserStatusId { get; set; }
        [Required]
        public int CollectionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("PrimarySessionId")]
        public UserSession PrimarySession { get; set; }
        [ForeignKey("UserStatusId")]
        public UserStatus UserStatus { get; set; }
        [ForeignKey("CollectionId")]
        public Collection Collection { get; set; }
    }
}
