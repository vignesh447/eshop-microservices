namespace EShop.Orders.Api.Api;

public record PlaceOrderRequest(Guid CustomerId, List<PlaceOrderRequestItem> Items);
public record PlaceOrderRequestItem(Guid ProductId, int Qty, decimal Price, string Currency = "USD");
public record OrderResponse(Guid Id, Guid CustomerId, string Status, decimal Total, string Currency, DateTime CreatedAtUtc);