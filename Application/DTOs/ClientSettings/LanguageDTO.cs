namespace Application.DTOs.ClientSettings;

public class LanguageDTO
{
    public required int Id { get; set; }
    public required string Locale { get; set; }
    public required string Name { get; set; }
    public required string NativeName { get; set; }
}

