namespace Archetype.Api.IntegrationTests.Support;

public abstract class IntegrationTestBase(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private bool _disposed;

    protected CustomWebApplicationFactory Factory { get; } = factory;
    protected HttpClient Client { get; } = factory.CreateClient();

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Client.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
