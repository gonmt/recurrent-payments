using Payments.Api.Extensions;
using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Infrastructure;
using Payments.Core.Users.Infrastructure;
using Payments.Core.Users.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.IncludeFields = true);

builder.Services.AddScoped<IHasher, BCryptHasher>();
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();

builder.Services.AddPaymentsCore();
builder.Services.RegisterApiEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapApiEndpoints();

app.Run();

public partial class Program;
