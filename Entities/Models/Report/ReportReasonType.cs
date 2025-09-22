using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models
{
    [Table("ReportReasonType")]
    public class ReportReasonType : BaseModel
    {
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