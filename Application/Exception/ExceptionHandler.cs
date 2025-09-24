using System.Net;
using Entities.Enums;

namespace Application.Exception
{

    public class ExceptionHandler
    {
        private readonly ExceptionBuilder _builder;
        private List<AppException> appExceptions = new();

        public ExceptionHandler()
        {
            _builder = new ExceptionBuilder();


            // Example of use of Decorator pattern
            AppException exceptionNotFound = _builder
                    .AddErrorId(ErrorType.NotFound)
                    .AddStatusCode(HttpStatusCode.NotFound)
                    .AddMessage("Not Found")
                    .Build();

            AppExceptionDecorator exceptionNotFoundDecorated = new(exceptionNotFound);
            exceptionNotFoundDecorated.ErrorId = ErrorType.NotFoundUser;
            exceptionNotFoundDecorated.Details = "Some details";
            exceptionNotFoundDecorated.Message = "Some Exception Message";
            //

            appExceptions.Add(
                _builder
                    .AddErrorId(ErrorType.BadRequest)
                    .AddStatusCode(HttpStatusCode.BadRequest)
                    .AddMessage("Bad request")
                    .Build());

            appExceptions.Add(
                _builder
                    .AddErrorId(ErrorType.Unauthorized)
                    .AddStatusCode(HttpStatusCode.Unauthorized)
                    .AddMessage("Unauthorized")
                    .Build());

            appExceptions.Add(
                _builder
                    .AddErrorId(ErrorType.PaymentRequired)
                    .AddStatusCode(HttpStatusCode.PaymentRequired)
                    .AddMessage("Payment required")
                    .Build());

            appExceptions.Add(
                _builder
                    .AddErrorId(ErrorType.Forbidden)
                    .AddStatusCode(HttpStatusCode.Forbidden)
                    .AddMessage("Forbidden")
                    .Build());

            appExceptions.Add(
                _builder
                    .AddErrorId(ErrorType.NotFound)
                    .AddStatusCode(HttpStatusCode.NotFound)
                    .AddMessage("Not Found")
                    .Build());

            appExceptions.Add(
                _builder
                    .AddErrorId(ErrorType.MethodNotAllowed)
                    .AddStatusCode(HttpStatusCode.MethodNotAllowed)
                    .AddMessage("Method Not Allowed")
                    .Build());

            appExceptions.Add(exceptionNotFound);
            appExceptions.Add(exceptionNotFoundDecorated);


            appExceptions.Add(_builder.AddStatusCode(HttpStatusCode.InternalServerError).AddMessage("Internal server error").Build());
        }

        public AppException GetExceptionResponse(ErrorType ErrorId)
        {
            AppException? appException = appExceptions.FirstOrDefault(er => er.ErrorId == ErrorId);
            if (appException != null)
            {
                return appException;
            }
            return appExceptions.Last();
        }
    }
}
