namespace EShop.Orders.Api.Domain.Aggregates.OrderAggregate;

public enum OrderStatus { Draft = 0, Submitted = 1, Paid = 2, Confirmed = 3, Cancelled = 9 }