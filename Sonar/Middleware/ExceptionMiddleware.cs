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

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        if (ex is not AppException appException) return;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)appException.StatusCode;
        string json = JsonSerializer.Serialize(appException);
        await context.Response.WriteAsync(json);
    }
}