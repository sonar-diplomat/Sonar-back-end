using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

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
    /// </summary>
    [ForeignKey("ReportableEntityTypeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual ReportableEntityType ReportableEntityType { get; set; }

    [ForeignKey("ReporterId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Reporter { get; set; }

    public virtual ICollection<ReportReasonType> ReportReasonType { get; set; }
    public virtual ICollection<Suspension> Suspensions { get; set; }
}
