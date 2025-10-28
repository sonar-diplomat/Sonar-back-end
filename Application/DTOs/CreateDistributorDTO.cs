using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public class CreateDistributorDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ContactEmail { get; set; }
    public DateTime ExpirationDate { get; set; }
    public IFormFile Cover { get; set; }
}