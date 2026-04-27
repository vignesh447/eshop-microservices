using Serilog;


using EShop.Notifications.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog(lc => lc
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Service", "Notifications")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
