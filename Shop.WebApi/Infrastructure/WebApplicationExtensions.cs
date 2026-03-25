using System.Reflection;

namespace Shop.WebApi.Infrastructure;

public static class WebApplicationExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && t.IsAssignableTo(typeof(IEndpointGroup)));

        foreach (var type in endpointGroupTypes)
        {
            var groupName = type.Name.Replace("Endpoints", string.Empty);
            
            var urlName = groupName.ToLower();

            var routePrefix = type.GetProperty(nameof(IEndpointGroup.RoutePrefix))
                    ?.GetValue(null) as string ?? $"/api/v1/{urlName}";
                    
            var group = app.MapGroup(routePrefix).WithTags(groupName);
            
            type.GetMethod(nameof(IEndpointGroup.Map))!.Invoke(null, [group]);
        }

        return app;
    }
}