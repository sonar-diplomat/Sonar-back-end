using System.Text.Json;
using Application.Exception;

namespace Sonar.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;


    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is IAppException)
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