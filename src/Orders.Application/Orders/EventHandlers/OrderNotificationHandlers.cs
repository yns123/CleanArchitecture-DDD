using Microsoft.Extensions.Logging;
using Orders.Application.Abstractions;
using Orders.Domain.Orders.Events;

namespace Orders.Application.Orders.EventHandlers;

// Örnek dinleyiciler: gerçek e-posta yerine log. PlaceOrderHandler bunlardan habersiz.
public sealed class LogOrderPlacedHandler(ILogger<LogOrderPlacedHandler> logger) : IDomainEventHandler<OrderPlaced>
{
    public Task HandleAsync(OrderPlaced e, CancellationToken ct)
    {
        logger.LogInformation("[E-posta] Müşteri {CustomerId}: {OrderId} numaralı siparişiniz alındı. Tutar: {Total} {Currency}",
            e.CustomerId, e.OrderId, e.Total.Amount, e.Total.Currency);
        return Task.CompletedTask;
    }
}

public sealed class LogOrderCancelledHandler(ILogger<LogOrderCancelledHandler> logger) : IDomainEventHandler<OrderCancelled>
{
    public Task HandleAsync(OrderCancelled e, CancellationToken ct)
    {
        logger.LogInformation("[E-posta] Müşteri {CustomerId}: {OrderId} numaralı siparişiniz iptal edildi.",
            e.CustomerId, e.OrderId);
        return Task.CompletedTask;
    }
}
