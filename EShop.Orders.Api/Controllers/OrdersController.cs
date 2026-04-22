using EShop.Orders.Api.Api;
using EShop.Orders.Api.Application.Abstractions;
using EShop.Orders.Api.Domain.Aggregates.OrderAggregate;
using EShop.Orders.Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Orders.Api.Controllers;

[ApiController, Route("api/orders")]
public class OrdersController(IOrderRepository repo) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Place(PlaceOrderRequest req, CancellationToken ct)
    {
        var order = new Order(req.CustomerId);
        foreach (var i in req.Items)
            order.AddItem(i.ProductId, i.Qty, new Money(i.Price, i.Currency));
        order.Submit();
        await repo.AddAsync(order, ct);
        await repo.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, Map(order));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> Get(Guid id, CancellationToken ct)
    {
        var o = await repo.GetAsync(id, ct);
        return o is null ? NotFound() : Ok(Map(o));
    }

    private static OrderResponse Map(Order o) =>
        new(o.Id, o.CustomerId, o.Status.ToString(), o.Total.Amount, o.Total.Currency, o.CreatedAtUtc);
}