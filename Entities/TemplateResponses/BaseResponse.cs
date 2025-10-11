namespace Entities.TemplateResponses;

public class BaseResponse<T>
{
    public BaseResponse()
    {
    }

    public BaseResponse(T data, string? message = null)
    {
        Success = true;
        Data = data;
        Message = message;
    }

    public BaseResponse(string message)
    {
        Success = false;
        Message = message;
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}
