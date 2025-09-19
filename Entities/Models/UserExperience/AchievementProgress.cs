using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AchievementProgress")]
    public class AchievementProgress
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(500)]
        public string Value { get; set; }
        
        [Required]
        public int AchievementId { get; set; }
        [Required]
        public int UserId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("AchievementId")]
        public virtual Achievement Achievement { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}