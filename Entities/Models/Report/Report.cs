using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Access;
using Infrastructure;

namespace Entities.Models.Report;

[Table("Report")]
public class Report : BaseModel
{
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
    public virtual User.User Reporter { get; set; }
        
    public virtual ICollection<ReportReasonType> ReportReasonType { get; set; }
    public virtual ICollection<Suspension> Suspensions { get; set; }
}