using Orders.Application.Abstractions;
using Orders.Application.Common;
using Orders.Domain.Orders;

namespace Orders.Application.Orders;

public sealed record PlaceOrderLine(Guid ProductId, int Quantity);
public sealed record PlaceOrderCommand(Guid CustomerId, string Currency, IReadOnlyList<PlaceOrderLine> Lines);

// Use case = Application katmanının işi: orkestrasyon. İş kuralı YOK (onlar Order'da).
// Handler: katalogdan fiyatı al -> aggregate'i çağır -> kaydet -> event'leri dağıt.
public sealed class PlaceOrderHandler(
    IOrderRepository orders, IProductCatalog catalog, IUnitOfWork uow, IDomainEventDispatcher events)
{
    public async Task<OrderDto> HandleAsync(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var order = Order.Create(cmd.CustomerId, cmd.Currency);

        foreach (var line in cmd.Lines)
        {
            var product = await catalog.FindAsync(line.ProductId, ct)
                ?? throw new NotFoundException($"Ürün bulunamadı: {line.ProductId}");
            order.AddItem(product.Id, product.Name, product.Price, line.Quantity);
        }

        order.Place();
        await orders.AddAsync(order, ct);
        await uow.SaveChangesAsync(ct);

        // Kayıttan SONRA: sipariş gerçekten kalıcı olmadan kimseye "alındı" demeyiz.
        await events.DispatchAsync(order, ct);
        return OrderDto.From(order);
    }
}
