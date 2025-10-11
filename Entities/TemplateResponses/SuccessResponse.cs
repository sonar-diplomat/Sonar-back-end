namespace Entities.TemplateResponses;

public class SuccessResponse
{
    public SuccessResponse(string message = "Operation completed successfully.")
    {
        Message = message;
    }

    public bool Success => true;
    public string Message { get; set; }
}
