namespace ImageBox.Tests;

public abstract class TestSetup
{
    private IServiceProvider? _services;

    private IServiceProvider GetServiceProvider()
    {
        if (_services is not null) return _services;

        return _services = new ServiceCollection()
            .AddImageBox()
            .BuildServiceProvider();
    }

    public T GetService<T>() where T : notnull
    {
        return GetServiceProvider().GetRequiredService<T>();
    }
}