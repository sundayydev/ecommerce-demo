using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Shop.WebApi.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "live" })
            .AddNpgSql(config.GetConnectionString("DefaultConnection")!, name: "database", tags: new[] { "ready" });
        return services;
    }
    
    public static WebApplication MapCustomHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions()
        {
            Predicate = r => r.Tags.Contains("ready")
        });

        return app;
    }
}