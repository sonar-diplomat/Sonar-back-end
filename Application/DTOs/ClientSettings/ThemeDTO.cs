namespace Application.DTOs.ClientSettings;

public class ThemeDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
