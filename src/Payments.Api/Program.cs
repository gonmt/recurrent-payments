using Microsoft.EntityFrameworkCore;

using Payments.Api.Extensions;
using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Infrastructure;
using Payments.Core.Users.Domain;
using Payments.Core.Users.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.IncludeFields = true);

builder.Services.AddScoped<IHasher, BCryptHasher>();
builder.Services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("PaymentsUsers"));
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddPaymentsCore();
builder.Services.RegisterApiEndpoints();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

app.MapApiEndpoints();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    UsersDbContext context = services.GetRequiredService<UsersDbContext>();
    IHasher hasher = services.GetRequiredService<IHasher>();
    UsersDbContextSeeder.Seed(context, hasher);
}

app.Run();

public partial class Program;
