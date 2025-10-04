using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class TokenDTO
{
    [Required]
    public string Token { get; set; }
}