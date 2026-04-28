using EShop.BuildingBlocks.Logging;
using EShop.BuildingBlocks.Telemetry;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Service", "Gateway")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

builder.Services.AddEShopTelemetry("Gateway");

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        if (builder.Environment.IsDevelopment())
            handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
    });

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseEShopTelemetry();

app.MapGet("/", () => Results.Ok(new { service = "EShop Gateway", status = "up" }));
app.MapReverseProxy();

app.Run();