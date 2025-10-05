using System.Reflection;

namespace Application.Exception
{
    public class AppExceptionFactory
    {
        public T Create<T>(string[]? args = null)
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