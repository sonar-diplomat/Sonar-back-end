using Application.Exception;

namespace Application.Abstractions.Interfaces.Exception;

public interface IAppExceptionFactory<T> where T : IAppException
{
    T CreateBadRequest(string? details = null); // 400
    T CreateUnauthorized(string? details = null); // 401
    T CreatePaymentRequired(string? details = null); // 402
    T CreateForbidden(string? details = null); // 403
    T CreateNotFound(int? id = null); // 404
    T CreateMethodNotAllowed(string? details = null); // 405
    T CreateNotAcceptable(string? details = null); // 406
    T CreateProxyAuthenticationRequired(string? details = null); // 407
    T CreateRequestTimeout(string? details = null); // 408
    T CreateConflict(string? details = null); // 409
    T CreateGone(string? details = null); // 410
    T CreateLengthRequired(string? details = null); // 411
    T CreatePreconditionFailed(string? details = null); // 412
    T CreatePayloadTooLarge(string? details = null); // 413
    T CreateUriTooLong(string? details = null); // 414
    T CreateUnsupportedMediaType(string? details = null); // 415
    T CreateRangeNotSatisfiable(string? details = null); // 416
    T CreateExpectationFailed(string? details = null); // 417
    T CreateImATeapot(string? details = null); // 418
    T CreateCustom(string message);
}