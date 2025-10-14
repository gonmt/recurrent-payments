using Microsoft.Extensions.DependencyInjection.Extensions;
using Payments.Api.Endpoints;

namespace Payments.Api.Extensions;

using Microsoft.AspNetCore.Builder;

public static class MapEndpointExtensions
{
    public static IServiceCollection RegisterApiEndpoints(this IServiceCollection services)
    {
        var assembly = typeof(MapEndpointExtensions).Assembly;
        
        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IApiEndpoint)) && t is { IsClass: true, IsAbstract: false, IsInterface: false });
        
        var serviceDescriptors = endpointTypes
            .Select(type => ServiceDescriptor.Transient(typeof(IApiEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }
    
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IApiEndpoint>>();
        
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
