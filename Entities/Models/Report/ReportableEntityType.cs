using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Report;

[Table("ReportableEntityType")]
public class ReportableEntityType : BaseModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Report> Reports { get; set; }
}