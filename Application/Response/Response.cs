using System.Collections;

namespace Application.Response;

public abstract class Response(bool success, int statusCode, string message, string[]? args = null)
    : Exception(message)
{
    public int StatusCode => statusCode;

    public virtual Dictionary<string, object> GetSerializableProperties()
    {
        Dictionary<string, object> dict = new()
        {
            ["success"] = success,
            ["message"] = Message
        };
        if (args != null)
            dict.Add(success ? "details" : "errors", args.Length == 1 ? args[0] : args);
        return dict;
    }
}

public abstract class Response<T>(T data, bool success, int statusCode, string message, string[]? args = null)
    : Response(success, statusCode, message, args)
{
    public override Dictionary<string, object> GetSerializableProperties()
    {
        Dictionary<string, object> dict = base.GetSerializableProperties();
        if (data is ICollection)
        {
            ICollection? col = data as ICollection;
            if (col == null || col.Count == 0)
                dict.Add("data", "empty list");
            else dict.Add("data", data);
        }
        else if (data != null) dict.Add("data", data);
        return dict;
    }
}

public class OkResponse(string[]? args = null) : Response(true, 200, "Operation completed successfully", args);

public class OkResponse<T>(T data, string[]? args = null)
    : Response<T>(data, true, 200, "Operation completed successfully", args);

public class CreatedResponse(string[]? args = null) : Response(true, 201, "Resource created successfully", args);

public class CreatedResponse<T>(T data, string[]? args = null)
    : Response<T>(data, true, 201, "Resource created successfully", args);

public class AcceptedResponse(string[]? args = null) : Response(true, 202, "Request accepted successfully", args);

public class AcceptedResponse<T>(T data, string[]? args = null)
    : Response<T>(data, true, 202, "Request accepted successfully", args);

public class NoContentResponse(string[]? args = null) : Response(true, 204, "No content", args);

public class NoContentResponse<T>(T data, string[]? args = null) : Response<T>(data, true, 204, "No content", args);

public class BadRequestResponse(string[]? args = null) : Response(false, 400, "Bad Request", args);

public class UnauthorizedResponse(string[]? args = null) : Response(false, 401, "Unauthorized access", args);

public class PaymentRequiredResponse(string[]? args = null) : Response(false, 402, "Payment required", args);

public class ForbiddenResponse(string[]? args = null) : Response(false, 403, "Access is https://open.spotify.com/track/4cLjibpprf7pcFohP8O6b3?si=559a44d314b64acf", args);

public class NotFoundResponse(string[]? args = null) : Response(false, 404, "Not found", args);

public class MethodNotAllowedResponse(string[]? args = null) : Response(false, 405, "Method not allowed", args);

public class NotAcceptableResponse(string[]? args = null) : Response(false, 406, "Not acceptable", args);

public class ProxyAuthenticationRequiredResponse(string[]? args = null)
    : Response(false, 407, "Proxy authentication required", args);

public class RequestTimeoutResponse(string[]? args = null) : Response(false, 408, "Timed out", args);

public class ConflictResponse(string[]? args = null) : Response(false, 409, "Already exists", args);

public class GoneResponse(string[]? args = null) : Response(false, 410, "Gone", args);

public class LengthRequiredResponse(string[]? args = null) : Response(false, 411, "Length required", args);

public class PreconditionFailedResponse(string[]? args = null) : Response(false, 412, "Precondition failed", args);

public class ContentTooLargeResponse(string[]? args = null) : Response(false, 413, "Content too large", args);

public class UriTooLongResponse(string[]? args = null) : Response(false, 414, "URI too long", args);

public class UnsupportedMediaTypeResponse(string[]? args = null) : Response(false, 415, "Unsupported media type", args);

public class RangeNotSatisfiableResponse(string[]? args = null) : Response(false, 416, "Range not satisfiable", args);

public class ExpectationFailedResponse(string[]? args = null) : Response(false, 417, "Expectation failed", args);

/// <summary>RFC 2324 / 7168: 418 I'm a teapot.</summary>
public class ImATeapotResponse(string[]? args = null) : Response(false, 418, "I'm a teapot — cannot brew coffee", args);

public class UnprocessableContentResponse(string[]? args = null) : Response(false, 422, "Unprocessable content", args);

public class InternalServerErrorResponse(string[]? args = null) : Response(false, 500, "Internal server error", args);