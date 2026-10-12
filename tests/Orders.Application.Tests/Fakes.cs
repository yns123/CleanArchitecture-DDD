using Orders.Application.Abstractions;
using Orders.Domain.Orders;

namespace Orders.Application.Tests;

// Elle yazılmış fake'ler: mock kütüphanesi yok, port'ların ne yaptığı net görünsün.
public sealed class FakeOrderRepository : IOrderRepository, IUnitOfWork
{
    public Dictionary<Guid, Order> Store { get; } = [];
    public int SaveCount { get; private set; }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(Store.GetValueOrDefault(id));
    public Task AddAsync(Order order, CancellationToken ct) { Store[order.Id] = order; return Task.CompletedTask; }
    public Task SaveChangesAsync(CancellationToken ct) { SaveCount++; return Task.CompletedTask; }
}

public sealed class FakeProductCatalog : IProductCatalog
{
    public Dictionary<Guid, ProductInfo> Products { get; } = [];
    public Task<ProductInfo?> FindAsync(Guid productId, CancellationToken ct) => Task.FromResult(Products.GetValueOrDefault(productId));
}
