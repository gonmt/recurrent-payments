using Archetype.Api.Endpoints.Auth;
using Archetype.Api.Endpoints.Shared;
using Archetype.Api.Extensions;
using Archetype.Core.Auth.Domain;
using Archetype.Core.Auth.Infrastructure;
using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Infrastructure;
using Archetype.Core.Users.Domain;
using Archetype.Core.Users.Infrastructure;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.IncludeFields = true);

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection(JwtTokenOptions.SectionName));
builder.Services.AddSingleton<ITokenProvider, JwtTokenProvider>();
builder.Services.AddScoped<IHasher, BCryptHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("ArchetypeUsers"));
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<QueryProcessor>();

builder.Services.AddArchetypeCore();
builder.Services.RegisterApiEndpoints();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

app.MapApiEndpoints();
await app.RunAsync();

public partial class Program
{
    protected Program()
    {
    }
}
