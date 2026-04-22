using EShop.Orders.Api.Domain.ValueObjects;

namespace EShop.Orders.Api.Domain.Aggregates.OrderAggregate;

public class OrderLine
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProductId { get; private set; }
    public int Qty { get; private set; }
    public Money Price { get; private set; }
    public Money LineTotal => new(Price.Amount * Qty, Price.Currency);

    private OrderLine() { } // EF
    public OrderLine(Guid productId, int qty, Money price)
    {
        if (qty <= 0) throw new ArgumentException("Qty must be > 0");
        ProductId = productId; Qty = qty; Price = price;
    }
}