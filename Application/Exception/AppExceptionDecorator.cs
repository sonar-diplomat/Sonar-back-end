namespace Application.Exception
{
    public class AppExceptionDecorator : AppException
    {
        public AppExceptionDecorator(AppException appException)
        {
            Message = appException.Message;
            Details = appException.Details;
        }

        public override string Message
        {
            get => base.Message;
            set => base.Message = value;
        }

        public override string? Details
        {
            get => base.Details;
            set => base.Details = value;
        }
    }
}
