using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Application.Common;
using Orders.Domain.Common;
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

// Aldığı event'leri kaydeden dinleyici.
public sealed class RecordingEventHandler<TEvent> : IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    public List<TEvent> Received { get; } = [];
    public Task HandleAsync(TEvent domainEvent, CancellationToken ct) { Received.Add(domainEvent); return Task.CompletedTask; }
}

// Gerçek DomainEventDispatcher'ı, kayıt tutan dinleyicilerle kurar.
public static class TestDispatcher
{
    public static DomainEventDispatcher Create(params (Type EventType, object Handler)[] handlers)
    {
        var services = new ServiceCollection();
        foreach (var (eventType, handler) in handlers)
            services.AddSingleton(typeof(IDomainEventHandler<>).MakeGenericType(eventType), handler);
        return new DomainEventDispatcher(services.BuildServiceProvider());
    }
}
