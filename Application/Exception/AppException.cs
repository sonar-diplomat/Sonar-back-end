using System.Net;

namespace Application.Exception;

public abstract class AppException(HttpStatusCode httpStatusCode, string message) : System.Exception(message)
{
    public HttpStatusCode StatusCode { get; } = httpStatusCode;

    public virtual Dictionary<string, object> GetSerializableProperties()
    {
        return new Dictionary<string, object>
        {
            ["StatusCode"] = (int)StatusCode,
            ["Message"] = Message
        };
    }
}

public class BadRequestException(string[]? args = null) : AppException(HttpStatusCode.BadRequest, "Bad Request")
{
    public override Dictionary<string, object> GetSerializableProperties()
    {
        Dictionary<string, object> dict = base.GetSerializableProperties();
        if (args == null) return dict;
        dict.Add("Details", args[0]);
        return dict;
    }
}

public class UnauthorizedException(string[]? args = null)
    : AppException(HttpStatusCode.Unauthorized, args is null ? "Unauthorized access" : $"Unauthorized: {args[0]}");

public class PaymentRequiredException(string[]? args = null)
    : AppException(HttpStatusCode.PaymentRequired, args is null ? "Payment required" : $"Payment required: {args[0]}");

public class ForbiddenException(string[]? args = null)
    : AppException(HttpStatusCode.Forbidden, args is null ? "Access forbidden" : $"Forbidden: {args[0]}");

public class NotFoundException(string[]? args = null)
    : AppException(HttpStatusCode.NotFound, args is null ? "Not found" : $"{args[0]} with id {args[1]} not found");

public class UserNotFoundException(string[]? args = null)
    : NotFoundException(args);

public class MethodNotAllowedException(string[]? args = null)
    : AppException(HttpStatusCode.MethodNotAllowed,
        args is null ? "Method not allowed" : $"Method not allowed: {args[0]}");

public class NotAcceptableException(string[]? args = null)
    : AppException(HttpStatusCode.NotAcceptable, args is null ? "Not acceptable" : $"Not acceptable: {args[0]}");

public class ProxyAuthenticationRequiredException(string[]? args = null)
    : AppException(HttpStatusCode.ProxyAuthenticationRequired, args is null
        ? "Proxy authentication required"
        : $"Proxy authentication required: {args[0]}");

public class RequestTimeoutException(string[]? args = null)
    : AppException(HttpStatusCode.RequestTimeout, args is null ? "Timed out" : $"Request timeout: {args[0]}");

public class ConflictException(string[]? args = null)
    : AppException(HttpStatusCode.Conflict, args is null ? "Already exists" : $"{args[0]} conflict: {args[1]}");

public class GoneException(string[]? args = null)
    : AppException(HttpStatusCode.Gone, args is null ? "Is gone" : $"Gone: {args[0]}");

public class LengthRequiredException(string[]? args = null)
    : AppException(HttpStatusCode.LengthRequired, args is null ? "Length required" : $"Length required: {args[0]}");

public class PreconditionFailedException(string[]? args = null)
    : AppException(HttpStatusCode.PreconditionFailed,
        args is null ? "Precondition failed" : $"Precondition failed: {args[0]}");

public class PayloadTooLargeException(string[]? args = null)
    : AppException(HttpStatusCode.RequestEntityTooLarge,
        args is null ? "Payload too large" : $"Payload too large: {args[0]}");

public class UriTooLongException(string[]? args = null)
    : AppException(HttpStatusCode.RequestUriTooLong, args is null ? "URI too long" : $"URI too long: {args[0]}");

public class UnsupportedMediaTypeException(string[]? args = null)
    : AppException(HttpStatusCode.UnsupportedMediaType,
        args is null ? "Unsupported media type" : $"Unsupported media type: {args[0]}");

public class RangeNotSatisfiableException(string[]? args = null)
    : AppException(HttpStatusCode.RequestedRangeNotSatisfiable,
        args is null ? "Range not satisfiable" : $"Range not satisfiable: {args[0]}");

public class ExpectationFailedException(string[]? args = null)
    : AppException(HttpStatusCode.ExpectationFailed,
        args is null ? "Expectation failed" : $"Expectation failed: {args[0]}");

/// <summary>RFC 2324 / 7168: 418 I'm a teapot.</summary>
public class ImATeapotException(string[]? args = null) : AppException((HttpStatusCode)418,
    args is null ? "I'm a teapot — cannot brew coffee" : $"I'm a teapot: {args[0]}");

public class UnprocessableContentException(string[]? args = null)
    : AppException((HttpStatusCode)422, args is null ? "Unprocessable content" : $"Unprocessable content: {args[0]}");
