namespace Entities.TemplateResponses;

public class ErrorResponse
{
    public ErrorResponse(string error, string? details = null)
    {
        Error = error;
        Details = details;
    }

    public string Error { get; set; }
    public string? Details { get; set; }
}
