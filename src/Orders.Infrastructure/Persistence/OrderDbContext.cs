using Microsoft.EntityFrameworkCore;
using Orders.Application.Abstractions;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Persistence;

// DbContext zaten Unit of Work'tür; Application'ın IUnitOfWork port'unu da o karşılar.
public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();

    Task IUnitOfWork.SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
}
