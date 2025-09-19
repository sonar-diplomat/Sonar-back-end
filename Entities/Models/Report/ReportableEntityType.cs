using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("ReportableEntityType")]
    public class ReportableEntityType
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<Report> Reports { get; set; }
    }
}
