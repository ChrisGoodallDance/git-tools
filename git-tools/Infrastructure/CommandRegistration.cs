using System.Reflection;

namespace git_tools.Infrastructure;

internal static class CommandRegistration
{
    public static IEnumerable<MethodInfo> GetRegistrationMethods()
    {
        var types = typeof(Program).Assembly.GetTypes();
        return from t in types
            where t.IsClass && !t.IsAbstract
            let method = t.GetMethod("Register", BindingFlags.Static | BindingFlags.Public)
            where method != null
            select method;
    }
}