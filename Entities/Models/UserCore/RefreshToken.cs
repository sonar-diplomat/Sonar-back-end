namespace Entities.Models.UserCore;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);
}