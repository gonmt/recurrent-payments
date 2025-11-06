using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace Archetype.Api.IntegrationTests.Support;

internal static class IntegrationTestEnvironment
{
    private static readonly object _sync = new();
    private static bool _initialized;
    private static bool _available;
    private static string? _connectionString;
    private static Exception? _error;
    private static PostgreSqlTestcontainer? _container;
    private static int _referenceCount;

    internal static bool EnsureInitialized()
    {
        lock (_sync)
        {
            if (_initialized)
            {
                return _available;
            }

            _initialized = true;

            string? fromEnvironment = Environment.GetEnvironmentVariable("USERS_DATABASE_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(fromEnvironment))
            {
                _connectionString = fromEnvironment;
                _available = true;
                return true;
            }

            try
            {
                PostgreSqlTestcontainerConfiguration configuration = new()
                {
                    Database = "archetype_tests",
                    Username = "postgres",
                    Password = "postgres"
                };

                _container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                    .WithDatabase(configuration)
                    .WithImage("postgres:16-alpine")
                    .Build();

                _container.StartAsync().GetAwaiter().GetResult();
                _connectionString = _container.ConnectionString;
                _available = true;
            }
            catch (Exception ex)
            {
                _error = ex;
                _available = false;
            }

            return _available;
        }
    }

    internal static IDisposable Acquire()
    {
        EnsureInitialized();

        lock (_sync)
        {
            _referenceCount++;
            return new ReleaseHandle();
        }
    }

    private sealed class ReleaseHandle : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Release();
        }

        private static void Release()
        {
            lock (_sync)
            {
                if (_referenceCount > 0)
                {
                    _referenceCount--;
                    if (_referenceCount == 0 && _container != null)
                    {
                        try
                        {
                            _container.DisposeAsync().AsTask().GetAwaiter().GetResult();
                        }
                        catch
                        {
                            // Ignore disposal exceptions
                        }
                        finally
                        {
                            _container = null;
                            _connectionString = null;
                            _available = false;
                            _initialized = false;
                            _error = null;
                        }
                    }
                }
            }
        }
    }

    internal static bool IsAvailable
    {
        get
        {
            lock (_sync)
            {
                return _available;
            }
        }
    }

    internal static string? ConnectionString
    {
        get
        {
            lock (_sync)
            {
                return _connectionString;
            }
        }
    }

    internal static Exception? Error
    {
        get
        {
            lock (_sync)
            {
                return _error;
            }
        }
    }
}
