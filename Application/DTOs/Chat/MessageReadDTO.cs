using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Chat;

public class MessageReadDTO
{
    public DateTime? ReadAt { get; set; }

    [Required]
    public int MessageId { get; set; }

    [Required]
    public int UserId { get; set; }
}

