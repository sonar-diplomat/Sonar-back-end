namespace Entities.TemplateResponses;

public class BaseResponse<T>
{
    public BaseResponse(T data, string? message = null)
    {
        Success = true;
        Data = data;
        Message = message;
        Metadata = new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss") }
        };
    }

    public BaseResponse(string message)
    {
        Success = false;
        Message = message;
        Metadata = new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss") }
        };
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public Dictionary<string, object> Metadata { get; set; }
}
