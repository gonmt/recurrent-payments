using Archetype.Core.Users.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Archetype.Api.Support;

internal sealed class TestingDatabaseMigrationService(IServiceProvider services) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        UsersDbContext context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        await context.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
