using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;

namespace Entities.Models.Access;

[Table("Suspension")]
public class Suspension : BaseModel
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; }

    [Required]
    public DateTime DateTime { get; set; }

    [Required]
    public int PunisherId { get; set; }

    [Required]
    public int AssociatedReportedId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("PunisherId")]
    public virtual User Punisher { get; set; }

    [ForeignKey("AssociatedReportId")]
    public virtual Report.Report AssociatedReport { get; set; }
}