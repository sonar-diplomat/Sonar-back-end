using Application.Response;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        if (ex is not Response appResponse)
        {
            // In development, let ASP.NET Core handle non-Response exceptions
            // to show the developer exception page
            if (environment.IsDevelopment())
            {
                throw ex;
            }

            // In production, return a generic error for non-Response exceptions
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            var genericError = new
            {
                StatusCode = 500,
                Message = "An internal server error occurred."
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