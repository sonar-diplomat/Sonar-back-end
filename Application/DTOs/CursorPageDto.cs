namespace Application.DTOs;

public class CursorPageDTO<T>
{
    public required IEnumerable<T> Items { get; set; }
    public string? NextCursor { get; set; }
}