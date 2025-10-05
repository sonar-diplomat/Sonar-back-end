using System.Reflection;

namespace Application.Exception
{
    public class AppExceptionFactory
    {
        //private static readonly string _errorSuffixPattern =
        //    "Exception|BadRequest|Unauthorized|PaymentRequired|Forbidden|NotFound|" +
        //    "MethodNotAllowed|NotAcceptable|ProxyAuthenticationRequired|RequestTimeout|" +
        //    "Conflict|Gone|LengthRequired|PreconditionFailed|PayloadTooLarge|UriTooLong|" +
        //    "UnsupportedMediaType|RangeNotSatisfiable|ExpectationFailed|ImATeapot|" +
        //    "InternalServerError|NotImplemented|BadGateway|ServiceUnavailable|GatewayTimeout|" +
        //    "HttpVersionNotSupported|VariantAlsoNegotiates|InsufficientStorage|LoopDetected|" +
        //    "NotExtended|NetworkAuthenticationRequired|Validation|Error";
        public T CreateCustom<T>(string[]? args = null)
        {
            return (T)Activator.CreateInstance(
                typeof(T),
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                args: args,
                culture: null
            )!;
        }
    }
}
