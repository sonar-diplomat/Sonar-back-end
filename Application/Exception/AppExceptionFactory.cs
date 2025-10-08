using System.Reflection;

namespace Application.Exception;

public class AppExceptionFactory
{
    public static T Create<T>(string[]? args = null) where T : AppException
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            args,
            null
        )!;
    }
}
