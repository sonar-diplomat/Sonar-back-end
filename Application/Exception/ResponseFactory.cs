using System.Reflection;

namespace Application.Exception;

public static class ResponseFactory
{
    public static T Create<T>(string[]? args = null) where T : Response
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            [args],
            null
        )!;
    }
    
    public static T Create<T>(object data, string[]? args = null) where T : Response
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            [data, args],
            null
        )!;
    }
}