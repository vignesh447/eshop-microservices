using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EShop.BuildingBlocks.Telemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddEShopTelemetry(
        this IServiceCollection services,
        string serviceName,
        bool includeSqlClient = false)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(
                serviceName: serviceName,
                serviceVersion: "1.0.0"))
            .WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation();
                t.AddHttpClientInstrumentation();

                if (includeSqlClient)
                    t.AddSqlClientInstrumentation();

                t.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:4317");
                });
            })
            .WithMetrics(m =>
            {
                m.AddAspNetCoreInstrumentation();
                m.AddHttpClientInstrumentation();
                m.AddRuntimeInstrumentation();
                m.AddPrometheusExporter();
            });

        return services;
    }

    public static IApplicationBuilder UseEShopTelemetry(this IApplicationBuilder app)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        return app;
    }
}