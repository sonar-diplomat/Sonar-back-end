using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public int ReportReasonTypeId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("ReportableEntityTypeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual ReportableEntityType ReportableEntityType { get; set; }

    [JsonIgnore]
    [ForeignKey("ReporterId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Reporter { get; set; }

    [JsonIgnore]
    [ForeignKey("ReportReasonTypeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual ReportReasonType ReportReasonType { get; set; }

    [JsonIgnore]
    public virtual ICollection<Suspension> Suspensions { get; set; }
}
