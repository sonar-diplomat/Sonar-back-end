using System.Text.Json;
using Application.Exception;

namespace Sonar.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is AppException)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                context.Response.StatusCode, exception.Message
            };

            string json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
