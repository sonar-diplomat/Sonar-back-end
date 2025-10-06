using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

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
    public int AssociatedReportId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("PunisherId")]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual User Punisher { get; set; }

    [ForeignKey("AssociatedReportId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Report.Report AssociatedReport { get; set; }
}