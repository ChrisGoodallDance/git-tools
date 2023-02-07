namespace git_tools.Infrastructure;

public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void Dispose()
    {
        if (_provider is IDisposable provider) provider.Dispose();
    }

    public object? Resolve(Type? type)
    {
        return type is null ? null : _provider.GetService(type);
    }
}