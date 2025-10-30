using Archetype.Api.Endpoints.Auth;
using Archetype.Api.Endpoints.Shared;
using Archetype.Api.Extensions;
using Archetype.Api.Middleware;
using Archetype.Api.Responses;
using Archetype.Core.Auth.Domain;
using Archetype.Core.Auth.Infrastructure;
using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Infrastructure;
using Archetype.Core.Users.Domain;
using Archetype.Core.Users.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Archetype.Api;

public class Program
{
    private static readonly string[] _generalArray = ["General"];

    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Archetype API",
                Version = "v1",
                Description = "Clean Architecture API Template with DDD principles",
                Contact = new OpenApiContact
                {
                    Name = "Archetype Template",
                    Email = "support@archetype.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            string xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.TagActionsBy(api =>
            {
                if (api.GroupName != null)
                {
                    return [api.GroupName];
                }

                HttpMethodAttribute? controllerAttribute = api.ActionDescriptor.EndpointMetadata
                    .OfType<HttpMethodAttribute>()
                    .FirstOrDefault();

                return controllerAttribute != null
                    ? [$"{controllerAttribute.Template?.Split('/').FirstOrDefault()}"]
                    : _generalArray;
            });

            options.OrderActionsBy(apiDescription => apiDescription.ActionDescriptor.AttributeRouteInfo?.Template ?? "");

            options.DocInclusionPredicate((_, _) => true);
        });

        builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.IncludeFields = true);

        builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection(JwtTokenOptions.SectionName));
        builder.Services.AddSingleton<ITokenProvider, JwtTokenProvider>();
        builder.Services.AddScoped<IHasher, BCryptHasher>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("ArchetypeUsers"));
        builder.Services.AddScoped<IUserRepository, EfUserRepository>();
        builder.Services.AddScoped<QueryProcessor>();
        builder.Services.AddScoped<ApiResponseWriter>();

        builder.Services.AddArchetypeCore();
        builder.Services.RegisterApiEndpoints();

        WebApplication app = builder.Build();

        app.UseMiddleware<CorrelationContextMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Archetype API v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "Archetype API Documentation";
                options.EnableDeepLinking();
                options.DisplayRequestDuration();
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });

            _ = app.MapOpenApi();
        }

        app.MapApiEndpoints();
        await app.RunAsync();
    }

    protected Program()
    {
    }
}
