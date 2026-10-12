using Orders.Application.Abstractions;
using Orders.Application.Common;

namespace Orders.Application.Orders;

public sealed record GetOrderQuery(Guid OrderId);

public sealed class GetOrderHandler(IOrderRepository orders)
{
    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken ct)
    {
        var order = await orders.GetByIdAsync(query.OrderId, ct)
            ?? throw new NotFoundException($"Sipariş bulunamadı: {query.OrderId}");
        return OrderDto.From(order);
    }
}
