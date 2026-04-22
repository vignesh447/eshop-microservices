namespace EShop.Orders.Api.Domain.ValueObjects;


public record Money(decimal Amount, string Currency = "USD")
{
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency) throw new InvalidOperationException("Currency mismatch");
        return new(a.Amount + b.Amount, a.Currency);
    }
    public static Money Zero(string currency = "USD") => new(0, currency);
}