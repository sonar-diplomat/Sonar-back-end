using Application.Response;
using System.Runtime.ExceptionServices;
using System.Text.Json;

namespace Sonar.Middleware;

public class ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment)
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new ResponseJsonConverter() }
    };

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip exception middleware for OpenAPI endpoints to allow proper error pages in development
        if (context.Request.Path.StartsWithSegments("/openapi"))
        {
            await next(context);
            return;
        }

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Проверяем, что ответ еще не начат (например, если OnChallenge уже установил ответ)
            if (!context.Response.HasStarted)
            {
                await HandleExceptionAsync(context, ex);
            }
            else
            {
                // Если ответ уже начат, просто пробрасываем исключение дальше
                throw;
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Проверяем, что ответ еще не начат (например, если OnChallenge уже установил ответ)
        if (context.Response.HasStarted)
        {
            return;
        }
        
        if (ex is not Response appResponse)
        {
            if (environment.IsDevelopment())
            {

                ExceptionDispatchInfo.Capture(ex).Throw();
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            var genericError = new
            {
                StatusCode = 500,
                Message = /*"An internal server error occurred."*/ex.Message
            };
            string json = JsonSerializer.Serialize(genericError, SerializerOptions);
            await context.Response.WriteAsync(json);
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = appResponse.StatusCode;
        string json2 = JsonSerializer.Serialize(appResponse, SerializerOptions);
        await context.Response.WriteAsync(json2);
    }
}