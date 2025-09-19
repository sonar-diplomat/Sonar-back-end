using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("ReportReasonType")]
    public class ReportReasonType
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public TimeSpan RecommendedSuspensionDuration { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<Report> Reports { get; set; }
    }
}