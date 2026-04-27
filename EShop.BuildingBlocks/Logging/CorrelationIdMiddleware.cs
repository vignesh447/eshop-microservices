using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace EShop.BuildingBlocks.Logging;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string Header = "X-Correlation-Id";

    public async Task Invoke(HttpContext ctx)
    {
        var cid = ctx.Request.Headers.TryGetValue(Header, out var v) && !string.IsNullOrWhiteSpace(v)
            ? v.ToString()
            : Guid.NewGuid().ToString("N");

        ctx.Response.Headers[Header] = cid;
        using (LogContext.PushProperty("CorrelationId", cid))
            await next(ctx);
    }
}