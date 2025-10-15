using Microsoft.Extensions.DependencyInjection.Extensions;

using Payments.Api.Endpoints;

namespace Payments.Api.Extensions;

public static class MapEndpointExtensions
{
    public static IServiceCollection RegisterApiEndpoints(this IServiceCollection services)
    {
        System.Reflection.Assembly assembly = typeof(MapEndpointExtensions).Assembly;

        IEnumerable<Type> endpointTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IApiEndpoint)) && t is { IsClass: true, IsAbstract: false, IsInterface: false });

        ServiceDescriptor[] serviceDescriptors = [.. endpointTypes.Select(type => ServiceDescriptor.Transient(typeof(IApiEndpoint), type))];

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        IEnumerable<IApiEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IApiEndpoint>>();

        foreach (IApiEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
