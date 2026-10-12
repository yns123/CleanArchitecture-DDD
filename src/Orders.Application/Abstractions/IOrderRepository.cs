using Orders.Domain.Orders;

namespace Orders.Application.Abstractions;

// Port: arayüz Application'da, implementasyon Infrastructure'da (bağımlılık tersine çevrilir).
// Repository aggregate root başınadır: OrderItem için ayrı repository yok.
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
}
