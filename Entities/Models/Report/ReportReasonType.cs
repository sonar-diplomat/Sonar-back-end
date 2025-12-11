using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Report;

[Table("ReportReasonType")]
public class ReportReasonType : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    public TimeSpan RecommendedSuspensionDuration { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Report> Reports { get; set; }

    /// <summary>
    /// Entity types for which this reason is applicable
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ReportableEntityType> ApplicableEntityTypes { get; set; }
}
