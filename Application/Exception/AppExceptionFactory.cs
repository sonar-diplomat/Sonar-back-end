using System.Reflection;
using System.Text.RegularExpressions;
using Application.Abstractions.Interfaces.Exception;

namespace Application.Exception;

public class AppExceptionFactory<T> : IAppExceptionFactory<T> where T : IAppException
{
    private const string _errorSuffixPattern =
        "Exception|BadRequest|Unauthorized|PaymentRequired|Forbidden|NotFound|" +
        "MethodNotAllowed|NotAcceptable|ProxyAuthenticationRequired|RequestTimeout|" +
        "Conflict|Gone|LengthRequired|PreconditionFailed|PayloadTooLarge|UriTooLong|" +
        "UnsupportedMediaType|RangeNotSatisfiable|ExpectationFailed|ImATeapot|" +
        "InternalServerError|NotImplemented|BadGateway|ServiceUnavailable|GatewayTimeout|" +
        "HttpVersionNotSupported|VariantAlsoNegotiates|InsufficientStorage|LoopDetected|" +
        "NotExtended|NetworkAuthenticationRequired|Validation|Error";


    public T CreateBadRequest(string? details = null) // 400
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Bad request for {entity}"
            : $"Bad request: {details}";

        return CreateCustom(message);
    }

    public T CreateUnauthorized(string? details = null) // 401
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Unauthorized access to {entity}"
            : $"Unauthorized: {details}";

        return CreateCustom(message);
    }

    public T CreatePaymentRequired(string? details = null) // 402
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Payment required to access {entity}"
            : $"Payment required: {details}";

        return CreateCustom(message);
    }

    public T CreateForbidden(string? details = null) // 403
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Access to {entity} is forbidden"
            : $"Forbidden: {details}";

        return CreateCustom(message);
    }

    public T CreateNotFound(int? id = null) // 404
    {
        string entity = GetEntityName();
        string message = id is null
            ? $"{entity} not found"
            : $"{entity} with id {id} not found";

        return CreateCustom(message);
    }

    public T CreateMethodNotAllowed(string? details = null) // 405
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Method not allowed for {entity}"
            : $"Method not allowed: {details}";

        return CreateCustom(message);
    }

    public T CreateNotAcceptable(string? details = null) // 406
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"{entity} not acceptable"
            : $"Not acceptable: {details}";

        return CreateCustom(message);
    }

    public T CreateProxyAuthenticationRequired(string? details = null) // 407
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Proxy authentication required for {entity}"
            : $"Proxy authentication required: {details}";

        return CreateCustom(message);
    }

    public T CreateRequestTimeout(string? details = null) // 408
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Request for {entity} timed out"
            : $"Request timeout: {details}";

        return CreateCustom(message);
    }

    public T CreateConflict(string? details = null) // 409
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"{entity} already exists"
            : $"{entity} conflict: {details}";

        return CreateCustom(message);
    }

    public T CreateGone(string? details = null) // 410
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"{entity} is gone"
            : $"Gone: {details}";

        return CreateCustom(message);
    }

    public T CreateLengthRequired(string? details = null) // 411
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Length required for {entity}"
            : $"Length required: {details}";

        return CreateCustom(message);
    }

    public T CreatePreconditionFailed(string? details = null) // 412
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Precondition failed for {entity}"
            : $"Precondition failed: {details}";

        return CreateCustom(message);
    }

    public T CreatePayloadTooLarge(string? details = null) // 413
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Payload too large for {entity}"
            : $"Payload too large: {details}";

        return CreateCustom(message);
    }

    public T CreateUriTooLong(string? details = null) // 414
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"URI too long for {entity}"
            : $"URI too long: {details}";

        return CreateCustom(message);
    }

    public T CreateUnsupportedMediaType(string? details = null) // 415
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Unsupported media type for {entity}"
            : $"Unsupported media type: {details}";

        return CreateCustom(message);
    }

    public T CreateRangeNotSatisfiable(string? details = null) // 416
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Range not satisfiable for {entity}"
            : $"Range not satisfiable: {details}";

        return CreateCustom(message);
    }

    public T CreateExpectationFailed(string? details = null) // 417
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"Expectation failed for {entity}"
            : $"Expectation failed: {details}";

        return CreateCustom(message);
    }

    public T CreateImATeapot(string? details = null) // 418
    {
        string entity = GetEntityName();
        string message = details is null
            ? $"I'm a teapot — cannot brew coffee with {entity}"
            : $"I'm a teapot: {details}";

        return CreateCustom(message);
    }


    public T CreateCustom(string message)
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new object[] { message },
            null
        )!;
    }

    private string GetEntityName()
    {
        string name = typeof(T).Name;
        return Regex.Replace(name, _errorSuffixPattern, "");
    }
}