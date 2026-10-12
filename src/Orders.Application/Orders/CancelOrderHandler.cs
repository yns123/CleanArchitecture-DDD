using Orders.Application.Abstractions;
using Orders.Application.Common;

namespace Orders.Application.Orders;

public sealed record CancelOrderCommand(Guid OrderId);

public sealed class CancelOrderHandler(IOrderRepository orders, IUnitOfWork uow, IDomainEventDispatcher events)
{
    public async Task HandleAsync(CancelOrderCommand cmd, CancellationToken ct)
    {
        var order = await orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw new NotFoundException($"Sipariş bulunamadı: {cmd.OrderId}");

        order.Cancel();
        await uow.SaveChangesAsync(ct);
        await events.DispatchAsync(order, ct);
    }
}
