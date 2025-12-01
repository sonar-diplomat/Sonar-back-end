namespace Application.DTOs.Search;

public class SearchUsersResultDTO
{
    public int Total { get; set; }
    public required IEnumerable<UserSearchItemDTO> Items { get; set; }
}

