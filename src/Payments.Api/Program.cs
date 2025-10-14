using Microsoft.EntityFrameworkCore;
using Payments.Api.Extensions;
using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Infrastructure;
using Payments.Core.Users.Domain;
using Payments.Core.Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.IncludeFields = true);

builder.Services.AddScoped<IHasher, BCryptHasher>();
builder.Services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("PaymentsUsers"));
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddPaymentsCore();
builder.Services.RegisterApiEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapApiEndpoints();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<UsersDbContext>();
    var hasher = services.GetRequiredService<IHasher>();
    UsersDbContextSeeder.Seed(context, hasher);
}

app.Run();

public partial class Program;
