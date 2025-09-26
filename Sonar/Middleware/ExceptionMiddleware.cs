using Application.ExceptionHandling;
using Entities.Enums;

namespace Sonar.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionHandler exceptionHandler;

        public ExceptionMiddleware(RequestDelegate next, ExceptionHandler exceptionHandler)
        {
            _next = next;
            this.exceptionHandler = exceptionHandler;
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
            if (exception.Data["ErrorType"] != null)
            {
                AppException appException = exceptionHandler.GetExceptionResponse((ErrorType)(exception.Data["ErrorType"])!);
                await CreateErrorResponse(context, appException);

                return;
            }

        }

        private async Task CreateErrorResponse(HttpContext context, AppException appException)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)appException.StatusCode;

            await context.Response.WriteAsJsonAsync(appException.ToJson);
        }
    }
}
