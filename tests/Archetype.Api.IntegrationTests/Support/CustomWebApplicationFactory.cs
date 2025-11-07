using Archetype.Core.Users.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Archetype.Api.IntegrationTests.Support;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string ConnectionStringKey = "ConnectionStrings:UsersDatabase";
    private readonly IDisposable _environmentHandle;

    internal static bool IsDatabaseConfigured => IntegrationTestEnvironment.IsAvailable;
    internal static Exception? InitializationError => IntegrationTestEnvironment.Error;

    public CustomWebApplicationFactory()
    {
        _environmentHandle = IntegrationTestEnvironment.Acquire();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        string? connectionString = IntegrationTestEnvironment.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [ConnectionStringKey] = connectionString
            });
        })
        .ConfigureServices(services =>
        {
            ServiceDescriptor? existingDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UsersDbContext>));

            if (existingDescriptor != null)
            {
                services.Remove(existingDescriptor);
            }

            services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName)));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        return !IsDatabaseConfigured
            ? new HostBuilder().Build()
            : base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _environmentHandle.Dispose();
        }
    }

}
