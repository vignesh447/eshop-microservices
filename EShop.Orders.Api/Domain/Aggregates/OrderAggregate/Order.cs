using EShop.Orders.Api.Domain.Exceptions;
using EShop.Orders.Api.Domain.ValueObjects;

namespace EShop.Orders.Api.Domain.Aggregates.OrderAggregate;

public class Order
{
    private readonly List<OrderLine> _lines = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Draft;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public IReadOnlyList<OrderLine> Lines => _lines;

    public Money Total =>
        _lines.Aggregate(Money.Zero(), (acc, l) => acc + l.LineTotal);

    private Order() { }
    public Order(Guid customerId) { CustomerId = customerId; }

    public void AddItem(Guid productId, int qty, Money price)
    {
        if (Status != OrderStatus.Draft) throw new DomainException("Cannot modify submitted order");
        _lines.Add(new OrderLine(productId, qty, price));
    }

    public void Submit()
    {
        if (_lines.Count == 0) throw new DomainException("Empty order");
        Status = OrderStatus.Submitted;
    }

    public void MarkPaid() => Status = OrderStatus.Paid;
    public void Confirm() => Status = OrderStatus.Confirmed;
    public void Cancel() => Status = OrderStatus.Cancelled;
}