using System.Net;

namespace Application.Exception;

public interface IAppException
{
    HttpStatusCode StatusCode { get; set; }
}

public abstract class AppException : System.Exception, IAppException
{
    public HttpStatusCode StatusCode { get; set; }

    public AppException() { }

    public AppException(string message, HttpStatusCode httpStatusCode)
        : base(message)
    {
        StatusCode = httpStatusCode;
    }
}

public class NotFoundUserException : AppException
{
    private NotFoundUserException(string message = "User not found") : base(message, HttpStatusCode.NotFound)
    {
    }

}

public class NotFoundTrackException : AppException
{
    private NotFoundTrackException(string message = "Track not found", HttpStatusCode StatusCode = HttpStatusCode.NotFound) : base(message, HttpStatusCode.NotFound)
    {
    }
}

public class NotFoundAlbumException : AppException
{
    private NotFoundAlbumException(string message = "User not found", HttpStatusCode StatusCode = HttpStatusCode.NotFound) : base(message, HttpStatusCode.NotFound)
    {
    }
}


