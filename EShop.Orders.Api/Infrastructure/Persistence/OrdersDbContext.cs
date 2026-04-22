using EShop.Orders.Api.Domain.Aggregates.OrderAggregate;
using EShop.Orders.Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Orders.Api.Infrastructure.Persistence;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<Order> order = modelBuilder.Entity<Order>();

        order.ToTable("Orders");
        order.HasKey(o => o.Id);
        order.Property(o => o.Status).HasConversion<int>();
        order.Ignore(o => o.Total);

        order.OwnsMany<OrderLine>(
            navigationExpression: (Order o) => o.Lines,
            buildAction: (OwnedNavigationBuilder<Order, OrderLine> lines) =>
            {
                lines.ToTable("OrderLines");
                lines.WithOwner().HasForeignKey("OrderId");
                lines.HasKey(l => l.Id);

                lines.OwnsOne<Money>(
                    navigationExpression: (OrderLine l) => l.Price,
                    buildAction: (OwnedNavigationBuilder<OrderLine, Money> price) =>
                    {
                        price.Property(m => m.Amount)
                             .HasColumnName("PriceAmount")
                             .HasColumnType("decimal(18,2)");
                        price.Property(m => m.Currency)
                             .HasColumnName("PriceCurrency")
                             .HasMaxLength(3);
                    });
            });
    }
}