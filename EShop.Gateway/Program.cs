var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        if (builder.Environment.IsDevelopment())
            handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
    });

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "EShop Gateway", status = "up" }));
app.MapReverseProxy();

app.Run();