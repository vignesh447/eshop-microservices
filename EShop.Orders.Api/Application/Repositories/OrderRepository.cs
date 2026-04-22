using EShop.Orders.Api.Application.Abstractions;
using EShop.Orders.Api.Domain.Aggregates.OrderAggregate;
using EShop.Orders.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EShop.Orders.Api.Infrastructure.Repositories;

public class OrderRepository(OrdersDbContext db) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await db.Orders.AddAsync(order, ct);

    public Task<Order?> GetAsync(Guid id, CancellationToken ct = default) =>
        db.Orders.Include("_lines").FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}