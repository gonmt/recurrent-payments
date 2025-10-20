using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Payments.Api.Endpoints.Auth;
using Payments.Api.Endpoints.Shared;
using Payments.Api.Extensions;
using Payments.Core.Auth.Domain;
using Payments.Core.Auth.Infrastructure;
using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Infrastructure;
using Payments.Core.Users.Domain;
using Payments.Core.Users.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.IncludeFields = true);

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection(JwtTokenOptions.SectionName));
builder.Services.AddSingleton<ITokenProvider, JwtTokenProvider>();
builder.Services.AddScoped<IHasher, BCryptHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("PaymentsUsers"));
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<QueryProcessor>();

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

await app.RunAsync();

public partial class Program
{
    // Protected constructor satisfies analyzers while keeping WebApplicationFactory support.
    protected Program()
    {
    }
}
