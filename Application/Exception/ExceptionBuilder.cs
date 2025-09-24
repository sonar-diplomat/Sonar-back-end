using System.Net;
using Entities.Enums;

namespace Application.Exception
{
    public class ExceptionBuilder
    {

        private AppException appException = new();

        public ExceptionBuilder()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.appException = new AppException();
        }

        public ExceptionBuilder AddErrorId(ErrorType errorId)
        {
            appException.ErrorId = errorId;
            return this;
        }

        public ExceptionBuilder AddStatusCode(HttpStatusCode statusCode)
        {
            appException.StatusCode = statusCode;
            return this;
        }

        public ExceptionBuilder AddMessage(string message)
        {
            appException.Message = message;
            return this;
        }

        public ExceptionBuilder AddDetails(string details)
        {
            appException.Details = details;
            return this;
        }


        public AppException Build()
        {
            AppException result = this.appException;

            this.Reset();

            return result;
        }
    }
}
