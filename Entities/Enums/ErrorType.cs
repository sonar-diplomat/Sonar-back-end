namespace Entities.Enums
{
    public enum ErrorType
    {
        NotFound,
        NotFoundUser,

        BadRequest,
        BadRequestUser,

        Unauthorized,

        PaymentRequired,

        Forbidden,

        MethodNotAllowed,

        InternalServerError
    }
}
