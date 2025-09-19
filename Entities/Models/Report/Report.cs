using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Report")]
    public class Report
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public bool IsClosed { get; set; }
        [Required] 
        public int EntityIdentifier { get; set; }
        
        [Required] 
        public int ReportableEntityTypeId { get; set; }
        [Required] 
        public int ReporterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("ReportableEntityTypeId")] 
        public virtual ReportableEntityType ReportableEntityType { get; set; }
        [ForeignKey("ReporterId")] 
        public virtual User Reporter { get; set; }
        
        public virtual ICollection<ReportReasonType> ReportReasonType { get; set; }
        public virtual ICollection<Suspension> Suspensions { get; set; }
    }
}