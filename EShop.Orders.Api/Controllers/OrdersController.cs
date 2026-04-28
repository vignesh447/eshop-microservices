using Asp.Versioning;
using EShop.Orders.Api.Api;
using EShop.Orders.Api.Application.Abstractions;
using EShop.Orders.Api.Domain.Aggregates.OrderAggregate;
using EShop.Orders.Api.Domain.ValueObjects;
using EShop.Payments.Grpc;
using Microsoft.AspNetCore.Mvc;
namespace EShop.Orders.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController(
    IOrderRepository repo,
    PaymentsService.PaymentsServiceClient payments,
    ILogger<OrdersController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Place(PlaceOrderRequest req, CancellationToken ct)
    {
        var order = new Order(req.CustomerId);
        foreach (var i in req.Items)
            order.AddItem(i.ProductId, i.Qty, new Money(i.Price, i.Currency));
        order.Submit();

        // Authorize via Payments over gRPC
        var auth = await payments.AuthorizeAsync(new AuthorizeRequest
        {
            OrderId = order.Id.ToString(),
            CustomerId = order.CustomerId.ToString(),
            Amount = (double)order.Total.Amount,
            Currency = order.Total.Currency
        }, cancellationToken: ct);

        if (auth.Status == AuthorizeReply.Types.Status.Approved)
        {
            order.MarkPaid();
            logger.LogInformation("Order {OrderId} authorized; paymentId={PaymentId}",
                order.Id, auth.PaymentId);
        }
        else
        {
            order.Cancel();
            logger.LogWarning("Order {OrderId} declined by Payments: {Reason}",
                order.Id, auth.Reason);
        }

        await repo.AddAsync(order, ct);
        await repo.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, Map(order));
    }

    [HttpGet("{id:guid}"), MapToApiVersion("1.0")]
    public async Task<ActionResult<OrderResponse>> Get(Guid id, CancellationToken ct)
    {
        var o = await repo.GetAsync(id, ct);
        return o is null ? NotFound() : Ok(Map(o));
    }

    [HttpGet("{id:guid}"), MapToApiVersion("2.0")]
    public async Task<ActionResult<object>> GetV2(Guid id, CancellationToken ct)
    {
        var o = await repo.GetAsync(id, ct);
        if (o is null) return NotFound();
        return Ok(new
        {
            o.Id,
            o.CustomerId,
            Status = o.Status.ToString(),
            Total = o.Total.Amount,
            Currency = o.Total.Currency,
            LinesCount = o.Lines.Count,
            o.CreatedAtUtc
        });
    }

    private static OrderResponse Map(Order o) =>
        new(o.Id, o.CustomerId, o.Status.ToString(), o.Total.Amount, o.Total.Currency, o.CreatedAtUtc);
}
