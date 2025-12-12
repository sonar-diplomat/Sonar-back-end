using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Report;

[Table("ReportableEntityType")]
public class ReportableEntityType : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Report> Reports { get; set; }

    /// <summary>
    /// Report reason types applicable to this entity type
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ReportReasonType> ApplicableReportReasonTypes { get; set; }
}
